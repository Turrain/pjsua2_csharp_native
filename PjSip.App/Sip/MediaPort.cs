using PjSua2.Lx;
using PjSua2.Native.pjsua2;

public class MediaPort : AudioMedia
{
    public event Action<byte[]> VoiceActivityDetected;
    private readonly VoiceActivityDetector _vad = new();

    public MediaPort()
    {
        _vad.VoiceSegmentDetected += frames => 
            VoiceActivityDetected?.Invoke(ProcessFrames(frames));
    }

    public void Transmit(byte[] audioData)
    {
        // Implementation depends on your audio processing
        // Example: Convert byte[] to audio frames and transmit
    }

    private byte[] ProcessFrames(ReadOnlyMemory<MediaFrame> frames)
    {
        return _vad.ExtractBytesFromFrames(frames.Span);
    }
}