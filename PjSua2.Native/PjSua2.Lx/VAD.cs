
using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using PjSua2.Lx.WebRtcVad;
using PjSua2.Native.pjsua2;

namespace PjSua2.Lx
{
    public sealed class VoiceActivityDetector : IDisposable
    {

        private readonly struct VadFrame
        {
            public readonly MediaFrame Frame;
            public readonly bool IsVoiced;
            public VadFrame(MediaFrame frame, bool isVoiced) => (Frame, IsVoiced) = (frame, isVoiced);
        }

        private sealed class RingBuffer
        {
            private readonly VadFrame[] _buffer;
            private int _head;
            private int _count;
            private int _voicedCount;

            public RingBuffer(int capacity) => _buffer = new VadFrame[capacity];

            public int Count => _count;
            public double VoiceRatio => _count > 0 ? (double)_voicedCount / _count : 0;

            public void Enqueue(VadFrame frame)
            {
                if (_count == _buffer.Length)
                {
                    if (_buffer[_head].IsVoiced)
                        _voicedCount--;
                    _head = (_head + 1) % _buffer.Length;
                }
                else
                {
                    _count++;
                }

                int tail = (_head + _count - 1) % _buffer.Length;
                _buffer[tail] = frame;
                if (frame.IsVoiced)
                    _voicedCount++;
            }

            public IEnumerable<VadFrame> GetFrames()
            {
                for (int i = 0; i < _count; i++)
                {
                    int index = (_head + i) % _buffer.Length;
                    yield return _buffer[index];
                }
            }

            public void Clear()
            {
                _head = 0;
                _count = 0;
                _voicedCount = 0;
                Array.Clear(_buffer, 0, _buffer.Length);
            }
        }

        public delegate void VoiceSegmentHandler(ReadOnlyMemory<MediaFrame> voiceFrames);
        public delegate void VoiceFrameHandler(MediaFrame frame, bool isVoiced);

        public event VoiceSegmentHandler? VoiceSegmentDetected;
        public event VoiceFrameHandler? VoiceFrameDetected;
        public event Action? SpeechStarted;
        public event Action? SilenceDetected;

        private readonly WebRtcVad.WebRtcVad _vad;
        private readonly RingBuffer _ringBuffer;
        private readonly List<MediaFrame> _voiceFrames;
        private readonly ArrayPool<byte> _arrayPool;
        private readonly object _lock = new();
        private bool _isSpeechActive;

        private const int PaddingMs = 160;
        private const int FrameDurationMs = 20;
        private const double SpeechStartRatio = 0.66;
        private const double SpeechEndRatio = 0.2;
        private const int MaxVoiceBufferSize = 1000;
          private const float NoiseThreshold = 500.0f;
        private static readonly int NumPaddingFrames = PaddingMs / FrameDurationMs;

        public VoiceActivityDetector(WebRtcVad.VadMode mode = WebRtcVad.VadMode.VeryAggressive)
        {
            _vad = new WebRtcVad.WebRtcVad();
            _vad.SetMode(mode);
            _ringBuffer = new RingBuffer(NumPaddingFrames);
            _voiceFrames = new List<MediaFrame>(MaxVoiceBufferSize);
            _arrayPool = ArrayPool<byte>.Shared;
        }

        /// <summary>
        /// Processes an incoming MediaFrame (which may have a reused buffer) and performs VAD.
        /// </summary>
        public void ProcessFrame(MediaFrame frame)
        {
            ArgumentNullException.ThrowIfNull(frame);

            lock (_lock)
            {
                if (IsFrameZeroFilled(frame))
                {
                    HandleSilentFrame(frame);
                    return;
                }

                // Create a temporary copy of the frame's data for VAD processing.
                byte[] frameData = frame.buf.ToArray();
                Span<byte> audioData = frameData;
                    PreprocessAudio(audioData);
                bool isVoiced = _vad.Process(VadSampleRate.Rate8KHz, audioData.ToArray()) == 1;

                if (!_isSpeechActive)
                {
                    HandlePotentialSpeechStart(frame, isVoiced);
                }
                else
                {
                    HandleActiveSpeech(frame, isVoiced);
                }

                VoiceFrameDetected?.Invoke(frame, isVoiced);
            }
        }

        /// <summary>
        /// Saves the accumulated media frames as a WAV file.
        /// </summary>
        public void SaveSegmentToWav(ReadOnlySpan<MediaFrame> frames, string path)
        {
            if (frames.IsEmpty)
                throw new ArgumentException("No frames provided", nameof(frames));

            // For 20ms at 8kHz with 16-bit (2-byte) samples, each frame is 320 bytes.
            int totalBytes = frames.Length * 320;
            byte[] buffer = _arrayPool.Rent(totalBytes);

            try
            {
                int offset = 0;
                foreach (var frame in frames)
                {
                    // Use a deep copy of the buffer.
                    ReadOnlySpan<byte> frameData = frame.buf.ToArray();
                    if (frameData.Length > 0)
                    {
                        frameData.CopyTo(buffer.AsSpan(offset));
                        offset += frameData.Length;
                    }
                }

                // Write only the valid PCM data.
                var pcmData = buffer.AsSpan(0, offset);
                WriteWavFile(MemoryMarshal.Cast<byte, short>(pcmData), path);
            }
            finally
            {
                _arrayPool.Return(buffer);
            }
        }

        /// <summary>
        /// Clears any stored state.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _ringBuffer.Clear();
                _voiceFrames.Clear();
                _isSpeechActive = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFrameZeroFilled(MediaFrame frame)
        {
            ReadOnlySpan<byte> data = frame.buf.ToArray();
            if (Vector.IsHardwareAccelerated && data.Length >= Vector<byte>.Count)
            {
                int vectorSize = Vector<byte>.Count;
                int vectorCount = data.Length / vectorSize;
                var zero = Vector<byte>.Zero;

                for (int i = 0; i < vectorCount; i++)
                {
                    if (!Vector.EqualsAll(new Vector<byte>(data.Slice(i * vectorSize)), zero))
                        return false;
                }

                return data.Slice(vectorCount * vectorSize).Contains((byte)0);
            }

            foreach (byte b in data)
                if (b != 0)
                    return false;

            return true;
        }

        private void HandleSilentFrame(MediaFrame frame)
        {
            if (_isSpeechActive)
            {
                EndSpeechSegment();
            }
            else
            {
                _ringBuffer.Enqueue(new VadFrame(frame, false));
            }
        }

        private void HandlePotentialSpeechStart(MediaFrame frame, bool isVoiced)
        {
            _ringBuffer.Enqueue(new VadFrame(frame, isVoiced));

            if (_ringBuffer.VoiceRatio >= SpeechStartRatio)
            {
                _isSpeechActive = true;
                SpeechStarted?.Invoke();
                _voiceFrames.Clear();
                // Deep copy frames from the ring buffer.
                foreach (var vadFrame in _ringBuffer.GetFrames())
                {
                    _voiceFrames.Add(CloneFrame(vadFrame.Frame));
                }
            }
        }

        private void HandleActiveSpeech(MediaFrame frame, bool isVoiced)
        {
            if (_voiceFrames.Count < MaxVoiceBufferSize)
                _voiceFrames.Add(CloneFrame(frame));

            _ringBuffer.Enqueue(new VadFrame(frame, isVoiced));

            if (_ringBuffer.VoiceRatio <= SpeechEndRatio)
                EndSpeechSegment();
        }

        private void EndSpeechSegment()
        {
            if (_voiceFrames.Count > 0)
            {
                VoiceSegmentDetected?.Invoke(_voiceFrames.ToArray());
                _voiceFrames.Clear();
            }

            _isSpeechActive = false;
            _ringBuffer.Clear();
            SilenceDetected?.Invoke();
        }

        public static void ChangeVolume(byte[] pcmData, float volumeFactor)
        {
            // Check that the array length is even (since each sample is 2 bytes)
            if (pcmData.Length % 2 != 0)
                throw new ArgumentException("PCM data length must be even.", nameof(pcmData));

            // Loop over the data, processing 2 bytes at a time
            for (int i = 0; i < pcmData.Length; i += 2)
            {
                // Convert 2 bytes to a short (16-bit signed integer)
                short sample = BitConverter.ToInt16(pcmData, i);
                int adjustedSample = (int)(sample * volumeFactor);
                if (adjustedSample > short.MaxValue)
                    adjustedSample = short.MaxValue;
                else if (adjustedSample < short.MinValue)
                    adjustedSample = short.MinValue;

                // Convert back to 2 bytes
                byte[] newSampleBytes = BitConverter.GetBytes((short)adjustedSample);
                pcmData[i] = newSampleBytes[0];
                pcmData[i + 1] = newSampleBytes[1];
            }
        }
        public byte[] ExtractBytesFromFrames(ReadOnlySpan<MediaFrame> frames)
        {
            if (frames.IsEmpty)
                throw new ArgumentException("No frames provided", nameof(frames));

            // Assume each frame is 320 bytes (20ms at 8kHz with 16-bit samples).
            int totalBytes = frames.Length * 320;
            byte[] rentedBuffer = _arrayPool.Rent(totalBytes);
            int offset = 0;

            try
            {
                foreach (var frame in frames)
                {
                    // Obtain a copy of the frame's buffer.
                    ReadOnlySpan<byte> frameData = frame.buf.ToArray();
                    if (frameData.Length > 0)
                    {
                        frameData.CopyTo(rentedBuffer.AsSpan(offset));
                        offset += frameData.Length;
                    }
                }

                // Copy only the valid bytes to a new array.
                byte[] extractedBytes = new byte[offset];
                Array.Copy(rentedBuffer, extractedBytes, offset);
                return extractedBytes;
            }
            finally
            {
                _arrayPool.Return(rentedBuffer);
            }
        }
        /// <summary>
        /// Creates a deep copy of a MediaFrame by copying its valid data.
        /// </summary>
        private MediaFrame CloneFrame(MediaFrame frame)
        {
            MediaFrame clone = new MediaFrame();
            clone.size = frame.size;
            // Deep copy the buffer so that the data remains available even if the original is reused.
            clone.buf = frame.buf;
            // Copy other properties if needed.
            return clone;
        }

        /// <summary>
        /// Writes a WAV file given 16-bit PCM samples.
        /// </summary>
        private static void WriteWavFile(ReadOnlySpan<short> samples, string path)
        {
            const int sampleRate = 8000;
            const short channels = 1;
            const short bitsPerSample = 16;
            const int byteRate = sampleRate * channels * bitsPerSample / 8;
            const short blockAlign = channels * bitsPerSample / 8;

            // Scale samples to maintain proper volume.
            const float scaleFactor = 0.8f;
            Span<short> scaledSamples = stackalloc short[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                float scaled = samples[i] * scaleFactor;
                scaledSamples[i] = (short)Math.Clamp(scaled, short.MinValue, short.MaxValue);
            }

            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: true);

            int dataLength = scaledSamples.Length * sizeof(short);
            int fileLength = 36 + dataLength;

            // RIFF header
            writer.Write("RIFF"u8);
            writer.Write(BitConverter.IsLittleEndian ? fileLength : BinaryPrimitives.ReverseEndianness(fileLength));
            writer.Write("WAVE"u8);

            // fmt chunk
            writer.Write("fmt "u8);
            writer.Write(BitConverter.IsLittleEndian ? 16 : BinaryPrimitives.ReverseEndianness(16));
            writer.Write((short)(BitConverter.IsLittleEndian ? 1 : BinaryPrimitives.ReverseEndianness((short)1))); // PCM
            writer.Write((short)(BitConverter.IsLittleEndian ? channels : BinaryPrimitives.ReverseEndianness(channels)));
            writer.Write(BitConverter.IsLittleEndian ? sampleRate : BinaryPrimitives.ReverseEndianness(sampleRate));
            writer.Write(BitConverter.IsLittleEndian ? byteRate : BinaryPrimitives.ReverseEndianness(byteRate));
            writer.Write((short)(BitConverter.IsLittleEndian ? blockAlign : BinaryPrimitives.ReverseEndianness(blockAlign)));
            writer.Write((short)(BitConverter.IsLittleEndian ? bitsPerSample : BinaryPrimitives.ReverseEndianness(bitsPerSample)));

            // data chunk
            writer.Write("data"u8);
            writer.Write(BitConverter.IsLittleEndian ? dataLength : BinaryPrimitives.ReverseEndianness(dataLength));

            // Write samples with explicit endianness handling.
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            foreach (short sample in scaledSamples)
            {
                if (BitConverter.IsLittleEndian)
                    BinaryPrimitives.WriteInt16LittleEndian(buffer, sample);
                else
                    BinaryPrimitives.WriteInt16BigEndian(buffer, sample);
                writer.Write(buffer);
            }
        }
 /// <summary>
        /// Preprocesses the raw PCM audio data to improve VAD performance.
        /// If the RMS energy is below a set threshold, the data is zeroed out.
        /// Otherwise, a high-pass filter is applied to reduce low-frequency noise.
        /// </summary>
        /// <param name="audioData">The PCM audio data as a Span&lt;byte&gt;.</param>
        private static void PreprocessAudio(Span<byte> audioData)
        {
            float rms = ComputeRms(audioData);
            if (rms < NoiseThreshold)
            {
                // Low energy: treat the frame as silence.
                audioData.Clear();
            }
            else
            {
                // Apply a high-pass filter to suppress low-frequency background noise.
                ApplyHighPassFilter(audioData);
            }
        }

        /// <summary>
        /// Computes the root-mean-square (RMS) value of the PCM audio data.
        /// </summary>
        /// <param name="audioData">The PCM audio data as a Span&lt;byte&gt;.</param>
        /// <returns>RMS energy as a float.</returns>
        private static float ComputeRms(Span<byte> audioData)
        {
            // Interpret the bytes as 16-bit signed samples.
            Span<short> samples = MemoryMarshal.Cast<byte, short>(audioData);
            double sumSquares = 0;
            foreach (short sample in samples)
            {
                sumSquares += sample * sample;
            }
            return (float)Math.Sqrt(sumSquares / samples.Length);
        }

        /// <summary>
        /// Applies a simple first-order high-pass filter to the PCM data.
        /// This helps remove low-frequency noise such as hum or background rumbles.
        /// </summary>
        /// <param name="audioData">The PCM audio data as a Span&lt;byte&gt;.</param>
        private static void ApplyHighPassFilter(Span<byte> audioData)
        {
            // Interpret the bytes as 16-bit signed samples.
            Span<short> samples = MemoryMarshal.Cast<byte, short>(audioData);
            if (samples.Length == 0)
                return;

            // High-pass filter parameters.
            const float cutoffFrequency = 80.0f; // in Hz
            const int sampleRate = 8000;
            float RC = 1.0f / (2 * MathF.PI * cutoffFrequency);
            float dt = 1.0f / sampleRate;
            float alpha = RC / (RC + dt);

            // Use a simple first-order high-pass filter:
            // y[n] = alpha * (y[n-1] + x[n] - x[n-1])
            short previousInput = samples[0];
            float previousOutput = samples[0];
            for (int i = 1; i < samples.Length; i++)
            {
                short currentInput = samples[i];
                float currentOutput = alpha * (previousOutput + currentInput - previousInput);
                samples[i] = (short)currentOutput;
                previousOutput = currentOutput;
                previousInput = currentInput;
            }
        }        public void Dispose()
        {
            _vad?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
