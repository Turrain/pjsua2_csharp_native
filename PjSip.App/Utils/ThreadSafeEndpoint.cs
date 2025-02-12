using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using PjSua2.Native.pjsua2;

namespace PjSip.App.Utils
{
    public class ThreadSafeEndpoint : IDisposable
    {
        private readonly Endpoint _endpoint;
        private readonly ILogger _logger;
        private readonly AsyncLocal<bool> _threadRegistered = new();
        private readonly object _initLock = new();
        private bool _isInitialized;

        public ThreadSafeEndpoint(ILogger logger)
        {
            _endpoint = new Endpoint();
            _logger = logger;
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            lock (_initLock)
            {
                if (_isInitialized) return;

                _endpoint.libCreate();
                var epConfig = new EpConfig
                {
                    logConfig = { level = 1 },
                    uaConfig = { maxCalls = 32 },
                    medConfig = { hasIoqueue = true }
                };
                

                _endpoint.libInit(epConfig);
                _endpoint.libStart();
                  _endpoint.audDevManager().setNullDev();
                _isInitialized = true;
            }
        }

        public T ExecuteSafely<T>(Func<T> action)
        {
            EnsureThreadRegistered();
            return action();
        }

        public void ExecuteSafely(Action action)
        {
            EnsureThreadRegistered();
            action();
        }

        private void EnsureThreadRegistered()
        {
            if (_threadRegistered.Value) return;

            try
            {
                if (!_endpoint.libIsThreadRegistered())
                {
                    var threadDesc = $"Thread-{Thread.CurrentThread.ManagedThreadId}";
                    _endpoint.libRegisterThread(threadDesc);
                    _threadRegistered.Value = true;
                    _logger.LogDebug("Registered thread {ThreadId} with PJSIP", threadDesc);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register thread with PJSIP");
                throw;
            }
        }

        public Endpoint Instance => _endpoint;

        public void Dispose()
        {
            try
            {
                EnsureThreadRegistered();
                if (_isInitialized)
                {
                    _endpoint.libDestroy();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing ThreadSafeEndpoint");
            }
        }
    }
}
