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
        }


        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            base.onCallMediaState(prm);
        }

        public Agent GetAgent()
        {
            return _account.GetAgent();
        }
    }
}