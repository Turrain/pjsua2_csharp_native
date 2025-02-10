using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{

    public class MediaPort : Native.pjsua2.AudioMediaPort
    {
        public VoiceActivityDetector _vad = new();
        private Queue<byte[]> _audioQueue = new Queue<byte[]>();
        // For 8kHz audio with 20ms frames:
        // 8000 samples/sec * 0.02 sec = 160 samples
        // 160 samples * 2 bytes/sample = 320 bytes
        private const int FRAME_SAMPLES = 160;
        private const int FRAME_BYTES = 160 * 2;
        private byte[] _pcmBuffer = null;
        private int _pcmBufferIndex = 0;

        public void AddToQueue(byte[] audioData)
        {
            if (audioData == null || audioData.Length == 0)
                return;

            lock (_audioQueue)
            {
                _audioQueue.Enqueue(audioData);
            }
        }


        public override void onFrameRequested(MediaFrame frame)
        {
            frame.type = pjmedia_frame_type.PJMEDIA_FRAME_TYPE_AUDIO;
            int requiredBytes = FRAME_BYTES; // 320 bytes for 20ms of 8kHz, 16-bit audio.
            byte[] tempBuffer = new byte[requiredBytes];
            int bytesCopied = 0;

            lock (_audioQueue)
            {
                // Copy bytes until we have the required amount.
                while (bytesCopied < requiredBytes)
                {
                    // If there is no current PCM buffer or we’ve exhausted it,
                    // try to get the next queued buffer.
                    if (_pcmBuffer == null || _pcmBufferIndex >= _pcmBuffer.Length)
                    {
                        if (_audioQueue.Count > 0)
                        {
                            _pcmBuffer = _audioQueue.Dequeue();
                            _pcmBufferIndex = 0;
                        }
                        else
                        {
                            // No more data available; exit the loop.
                            break;
                        }
                    }

                    int remainingBytes = requiredBytes - bytesCopied;
                    int availableBytes = _pcmBuffer.Length - _pcmBufferIndex;
                    int bytesToCopy = Math.Min(remainingBytes, availableBytes);

                    Array.Copy(_pcmBuffer, _pcmBufferIndex, tempBuffer, bytesCopied, bytesToCopy);
                    bytesCopied += bytesToCopy;
                    _pcmBufferIndex += bytesToCopy;
                }
            }

            // If we couldn't fill the whole frame, pad the rest with zeros.
            if (bytesCopied < requiredBytes)
            {
                for (int i = bytesCopied; i < requiredBytes; i++)
                {
                    tempBuffer[i] = 0;
                }
            }
            

            
            ByteVector bv = [.. tempBuffer];
            frame.buf = bv;
          //  Console.WriteLine("First few bytes: " + string.Join(", ", bv.Take(bv.Count)));
            frame.size = (uint)requiredBytes;
        }



        public override void onFrameReceived(MediaFrame frame)
        {


            // Ensure we have valid frame data before processing
            if (frame == null || frame.buf == null || frame.size == 0)
            {
                return; // Skip processing invalid frames
            }

            // Only process frames of the expected size

            // Console.WriteLine($"{frame.buf.ToArray()}");

            // // Convert the buffer to a hexadecimal string representation
            // StringBuilder hex = new StringBuilder(frame.buf.Count * 2);
            // foreach (byte b in frame.buf)
            // {
            //     hex.AppendFormat("{0:x2} ", b);
            // }
            // Console.WriteLine(hex.ToString());
      
            _vad.ProcessFrame(frame);


        }

        public void ClearQueue()
        {
            lock (_audioQueue)
            {
                _audioQueue.Clear();
            }
            _pcmBuffer = null;
            _pcmBufferIndex = 0;
        }
    }


   public static class MediaPortTest
{
    private static Timer _audioFeedTimer;
    private static byte[] _pcmData;
    private static int _feedPosition = 0;

    public static void RunTest(MediaPort mediaPort)
    {
        string filePath = "test.wav";
        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        if (!File.Exists(filePath))
        {
            Console.WriteLine("WAV file not found: " + filePath);
            return;
        }

        try
        {
            byte[] wavData = File.ReadAllBytes(filePath);
            int headerSize = 44;
            _pcmData = new byte[wavData.Length - headerSize];
            Array.Copy(wavData, headerSize, _pcmData, 0, _pcmData.Length);

            // Feed data in chunks every 20ms to simulate real-time
            _audioFeedTimer = new Timer(FeedData, mediaPort, 0, 20);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }

    private static void FeedData(object state)
    {
        MediaPort mediaPort = (MediaPort)state;
        int chunkSize = 320; // Match FRAME_BYTES

        lock (_pcmData)
        {
            if (_feedPosition >= _pcmData.Length)
            {
                _feedPosition = 0; // Loop or stop
            }

            int bytesRemaining = _pcmData.Length - _feedPosition;
            int bytesToCopy = Math.Min(chunkSize, bytesRemaining);
            byte[] chunk = new byte[bytesToCopy];
            Array.Copy(_pcmData, _feedPosition, chunk, 0, bytesToCopy);
            _feedPosition += bytesToCopy;

            mediaPort.AddToQueue(chunk);
        }
    }
}
}
