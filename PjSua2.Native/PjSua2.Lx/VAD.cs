using System;
using System.Collections.Generic;
using PjSua2.Native;
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
    private Queue<Tuple<short[], bool>> _vadRingBuffer = new Queue<Tuple<short[], bool>>();
    private List<Native.pjsua2.MediaFrame> _voiceBuffer = new List<Native.pjsua2.MediaFrame>();
    private bool _triggered = false;

    public VoiceSegmentCallback OnVoiceSegment;
    public SilenceCallback OnSilence;
    public VoiceFrameCallback OnVoiceFrame;
    public SpeechStartedCallback OnSpeechStarted;

    public VAD()
    {
        _vad.OperatingMode = OperatingMode.Aggressive;
    }

    public void ProcessFrame(byte[] frame)
    {
        bool isVoiced = _vad.HasSpeech(frame, SampleRate.Is16kHz, FrameLength.Is20ms);
        lock (_bufferLock)
        {
            ProcessVAD(frame, isVoiced);
        }
    }

    private void ProcessVAD(byte[] frame, bool isVoiced)
    {
        // VAD processing logic here
    }
}

}