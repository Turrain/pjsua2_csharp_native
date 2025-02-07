using System;
using System.Collections.Generic;
using System.Threading;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{

    public class MediaPort : Native.pjsua2.AudioMediaPort
    {
        private VAD _vad = new VAD();
        private Queue<byte[]> _audioQueue = new Queue<byte[]>();
        private int _frameSize = 320;
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

            int requiredBytes = _frameSize;
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

            // Assign the filled (or padded) buffer to the frame.
            frame.buf = [.. tempBuffer];
            frame.size = (uint)requiredBytes;

            // Optionally, call the base implementation.
            base.onFrameRequested(frame);
        }


        public override void onFrameReceived(MediaFrame frame)
        {
            base.onFrameReceived(frame);
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
}