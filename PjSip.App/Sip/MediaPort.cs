using PjSua2.Lx;
using PjSua2.Native.pjsua2;

public class MediaPort : AudioMediaPort
{
    public event Action<byte[]> VoiceActivityDetected;
    public VoiceActivityDetector _vad = new();
    private Queue<byte[]> _audioQueue = new Queue<byte[]>();

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
        //Console.WriteLine("First few bytes: " + string.Join(", ", bv.Take(bv.Count)));
        frame.size = (uint)requiredBytes;
    }



    public override void onFrameReceived(MediaFrame frame)
    {

        if (frame == null || frame.buf == null || frame.size == 0)
        {
            return;
        }
        // Console.WriteLine($"{string.Join(",",frame.buf.ToArray().Take(frame.buf.ToArray().Length))}");

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