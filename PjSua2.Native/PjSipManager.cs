using System;
using PjSua2.Native.pjsua2;

namespace PjSua2.Native
{

    

    public class PjSipManager : IDisposable
    {

        
        private Endpoint _endpoint;
       private Account? _account;  
        private bool _disposed;

        public PjSipManager()
        {
            _endpoint = new Endpoint();
            _endpoint.libCreate();
        }

        public void Initialize()
        {
            // Create library configuration
            var epConfig = new EpConfig();
            
            // Initialize the library
            _endpoint.libInit(epConfig);
            
            // Create SIP transport
            var transportConfig = new TransportConfig();
            transportConfig.port = 5060;
            _endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, transportConfig);
            
            // Start the library
            _endpoint.libStart();
        }

        public void CreateAccount(string username, string password, string domain)
        {
            // Create account configuration
            var accConfig = new AccountConfig();
            accConfig.idUri = $"sip:{username}@{domain}";
            accConfig.regConfig.registrarUri = $"sip:{domain}";

            // Add credentials
            var cred = new AuthCredInfo("digest", "*", username, 0, password);
            accConfig.sipConfig.authCreds.Add(cred);

            // Create account
            _account = new Account();
            _account.create(accConfig);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_account != null)
                {
                    _account.Dispose();
                }

                if (_endpoint != null)
                {
                    _endpoint.libDestroy();
                    _endpoint.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
