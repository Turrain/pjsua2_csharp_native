using System;
using System.Collections.Generic;
using PjSua2.Native.pjsua2;
using WebRtcVadSharp;
namespace PjSua2.Lx
{
    public class VAD
    {
        public delegate void VoiceSegmentCallback(List<Native.pjsua2.MediaFrame> frames);
        public delegate void SilenceCallback();
        public delegate void VoiceFrameCallback(Native.pjsua2.MediaFrame frame);
        public delegate void SpeechStartedCallback();

        private WebRtcVad _vad = new WebRtcVad();
        private object _bufferLock = new object();
        private Queue<Tuple<MediaFrame, bool>> _vadRingBuffer = new Queue<Tuple<MediaFrame, bool>>();
        private List<MediaFrame> _voiceBuffer = new List<MediaFrame>();
        private bool _triggered = false;
       
        private const int PADDING_MS = 300;
        private const int FRAME_DURATION_MS = 20;
        private const int NUM_PADDING_FRAMES = PADDING_MS / FRAME_DURATION_MS;  // e.g. 15 frames
        private const double VAD_RATIO = 0.9;
        private const int MAX_BUFFER_SIZE = 500;  // adjust as needed


        public VoiceSegmentCallback OnVoiceSegment;
        public SilenceCallback OnSilence;
        public VoiceFrameCallback OnVoiceFrame;
        public SpeechStartedCallback OnSpeechStarted;

        public VAD()
        {
            _vad.OperatingMode = OperatingMode.Aggressive;
        }

        public void ProcessFrame(MediaFrame frame)
        {
            bool isVoiced = _vad.HasSpeech(frame.buf.ToArray(), SampleRate.Is16kHz, FrameLength.Is20ms);
            lock (_bufferLock)
            {
                ProcessVAD(frame, isVoiced);
            }
        }

        private void ProcessVAD(MediaFrame frame, bool isVoiced)
        {
             if (!_triggered)
            {
                // Not yet in a speech segment: add to the ring buffer.
                _vadRingBuffer.Enqueue(Tuple.Create(frame, isVoiced));
                if (_vadRingBuffer.Count > NUM_PADDING_FRAMES)
                {
                    _vadRingBuffer.Dequeue();
                }

                // Count the number of voiced frames in the ring buffer.
                int numVoiced = _vadRingBuffer.Count(pair => pair.Item2);
                if (numVoiced > VAD_RATIO * _vadRingBuffer.Count)
                {
                    // Enter speech segment.
                    _triggered = true;
                    _voiceBuffer.Clear();
                    OnSpeechStarted?.Invoke();

                    // Process any buffered frames as part of the speech segment.
                    foreach (var pair in _vadRingBuffer)
                    {
                        ProcessVoicedFrame(pair.Item1);
                    }
                    _vadRingBuffer.Clear();
                }
            }
            else
            {
                // Already in speech: process the current frame.
                ProcessVoicedFrame(frame);

                _vadRingBuffer.Enqueue(Tuple.Create(frame, isVoiced));
                if (_vadRingBuffer.Count > NUM_PADDING_FRAMES)
                {
                    _vadRingBuffer.Dequeue();
                }

                // Count unvoiced frames in the ring buffer.
                int numUnvoiced = _vadRingBuffer.Count(pair => !pair.Item2);
                if (numUnvoiced > VAD_RATIO * _vadRingBuffer.Count)
                {
                    // End of speech: notify the client and reset buffers.
                    if (OnVoiceSegment != null && _voiceBuffer.Count > 0)
                    {
                        OnVoiceSegment(_voiceBuffer);
                    }
                    _triggered = false;
                    ProcessSilence();
                    _voiceBuffer.Clear();
                    _vadRingBuffer.Clear();
                }
            }
        }
  
        private void ProcessVoicedFrame(MediaFrame frame)
        {
            if (_voiceBuffer.Count < MAX_BUFFER_SIZE)
            {
                _voiceBuffer.Add(frame);
            }
            OnVoiceFrame?.Invoke(frame);
        }

 
        private void ProcessSilence()
        {
            OnSilence?.Invoke();
        }
        public short[] MergeFrames(List<MediaFrame> frames)
        {
            // Calculate total number of samples (assuming 16-bit samples)
            int totalSamples = (int)frames.Sum(f => f.size / 2);
            short[] result = new short[totalSamples];
            int offset = 0;
            foreach (var frame in frames)
            {
                // Convert each frame’s byte buffer into shorts.
                // Assumes little-endian encoding.
                for (int i = 0; i < frame.size; i += 2)
                {
                    result[offset++] = BitConverter.ToInt16([.. frame.buf], i);
                }
            }
            return result;
        }
    }

}