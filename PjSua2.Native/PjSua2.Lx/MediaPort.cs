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
        private byte[] _pcmBuffer;
        private int _pcmBufferIndex = 0;

        public void AddToQueue(byte[] audioData)
        {
            lock (_audioQueue)
            {
                _audioQueue.Enqueue(audioData);
            }
        }


        public override void onFrameRequested(MediaFrame frame)
        {
            base.onFrameRequested(frame);
          
        }


        public override void onFrameReceived(MediaFrame frame)
        {
            base.onFrameReceived(frame);
            _vad.ProcessFrame(frame.buf.ToArray());
        }

        public void ClearQueue()
        {
            lock (_audioQueue)
            {
                _audioQueue.Clear();
            }
        }
    }
}