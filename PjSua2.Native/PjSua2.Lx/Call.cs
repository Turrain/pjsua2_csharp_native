using System;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{
    public class Call : Native.pjsua2.Call
    {
        static int port_index = 0;
        public Call(Account acc, int callId = -1) : base(acc, callId)
        {
            this._account = acc;
            _mediaPort ??= new MediaPort();
            _mediaPort._vad.VoiceSegmentDetected += delegate (ReadOnlyMemory<MediaFrame> voiceFrames) {
                Console.WriteLine("SEGMENT " + voiceFrames.Length);
               //    _mediaPort._vad.SaveSegmentToWav(voiceFrames.Span, $"SEGMENT_{voiceFrames.Length}.wav");
                var data = _mediaPort._vad.ExtractBytesFromFrames(voiceFrames.Span);
                _account._agent.Listen(data);
                _mediaPort.ClearQueue();
            };
            _account._agent.auralisClient.OnBinaryMessage += data => {
                _mediaPort.AddToQueue(data);
            };
            _mediaPort._vad.VoiceFrameDetected += delegate (MediaFrame frame, bool isVoiced) {
          
            };
        
            if (_mediaPort.getPortId() == pjsua2.INVALID_ID)
            {
                var mediaFormatAudio = new MediaFormatAudio();
                mediaFormatAudio.init(
                    1,
                    8000,
                    1,
                    20000,
                    16
                );
                _mediaPort.createPort($"default{++port_index}", mediaFormatAudio);
            }

        }
        public enum Direction
        {
            Incoming,
            Outgoing
        }

        public Direction CallDirection { get; set; } = Direction.Outgoing;
        private Account _account;
        private MediaPort _mediaPort;


        public override void onCallState(OnCallStateParam prm)
        {
            base.onCallState(prm);
            // var ci = getInfo();

        }


        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            var ci = getInfo();
          
            for (var i = 0; i < ci.media.Count; i++)
            {
                if ((ci.media[i].status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE) && (ci.media[i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO))
                {
                    var aud_med = getAudioMedia(i);

                    var aud_dev_man = Endpoint.instance().audDevManager();
               //     var mic = Endpoint.instance().audDevManager().getCaptureDevMedia();
               //     var speaker = Endpoint.instance().audDevManager().getPlaybackDevMedia();
               
                    if (aud_med == null) continue;


                    if (CallDirection == Direction.Incoming)
                    {
                        Console.WriteLine("Incoming call from " + ci.remoteUri);
                        _ = _account._agent.Speak("Hello from agent");
                    //    mic.startTransmit(_mediaPort);
                     //  MediaPortTest.RunTest(_mediaPort);

                        //  var adplayer = new AudioMediaPlayer();
                        // string filePath = "test.wav";
                        // filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
                        // adplayer.createPlayer(filePath);
                        // adplayer.startTransmit(speaker);
                     

                    }
                  
                    aud_med.startTransmit(_mediaPort);
                    _mediaPort.startTransmit(aud_med);
          
                //     _mediaPort.startTransmit(speaker);




                }
            }
        }


    }
}