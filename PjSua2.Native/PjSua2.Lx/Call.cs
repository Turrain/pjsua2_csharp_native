using System;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{
    public class Call : Native.pjsua2.Call
    {

        public Call(Account acc, int callId = -1):base(acc, callId) {
             _account = acc;
             if(_mediaPort.getPortId() == pjsua2.INVALID_ID){
                var mediaFormatAudio = new MediaFormatAudio();
                mediaFormatAudio.init(
                    1,
                    8000,
                    1,
                    20000,
                    16
                );
                _mediaPort.createPort("default", mediaFormatAudio);
             }

        }
        public enum Direction
        {
            Incoming,
            Outgoing
        }

        public Direction CallDirection { get; private set; }
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
            for (var i = 0; i< ci.media.Count; i++){
                if((ci.media[i].status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE) && (ci.media[i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO )){
                    var aud_med = (AudioMedia)getMedia((uint)i);
                    var aud_dev_man = Endpoint.instance().audDevManager();
                    var port_info = aud_med.getPortInfo();
                    var format = port_info.format;
                    
                    aud_med.startTransmit(_mediaPort);
                    _mediaPort.startTransmit(aud_med);
                }
            }
        }

        public Agent GetAgent()
        {
            return _account.GetAgent();
        }
    }
}