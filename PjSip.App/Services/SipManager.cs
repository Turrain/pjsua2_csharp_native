using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PjSua2.Native.pjsua2;
using PjSip.App.Exceptions;
using PjSip.App.Utils;
using PjSip.App.Sip;
using PjSip.App.Data;

namespace PjSip.App.Services
{
    public class SipManager : IDisposable
    {
        private readonly ThreadSafeEndpoint _endpoint;
        private readonly BlockingCollection<Action> _taskQueue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _workerTask;
        private readonly SipDbContext _context;
        private readonly ILogger<SipManager> _logger;
        private readonly RetryPolicy _retryPolicy;
        private readonly ILoggerFactory _loggerFactory;

        // State containers
        private readonly ConcurrentDictionary<string, Sip.Account> _accounts = new();
        private readonly ConcurrentDictionary<int, (Sip.Call Call, string AccountId)> _activeCalls = new();
        private readonly CircuitBreaker _circuitBreaker;

        private const int MAX_RETRIES = 3;
        private const int RETRY_DELAY_MS = 1000;

        public SipManager(SipDbContext context, ILogger<SipManager> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _loggerFactory = loggerFactory;

            try
            {
                _endpoint = new ThreadSafeEndpoint(logger);
                _endpoint.Initialize();
                InitializeTransport();
                _endpoint.Instance.audDevManager().setNullDev();

                // Initialize retry policy and circuit breaker
                _retryPolicy = new RetryPolicy(MAX_RETRIES, RETRY_DELAY_MS);
                _circuitBreaker = new CircuitBreaker(
                    failureThreshold: 5,
                    resetTimeout: TimeSpan.FromMinutes(1)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize PJSUA2");
                throw new TransportException(
                    "Failed to initialize PJSIP stack", 
                    "UDP", 
                    5060, 
                    ex);
            }
            
            try
            {
                // Start worker thread
                _workerTask = Task.Run(ProcessTaskQueue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize manager");
                throw;
            }
        }
     
        private void InitializeTransport()
        {
            try
            {
                _endpoint.ExecuteSafely(() =>
                {
                    var transport = _endpoint.Instance.transportCreate(
                        pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                        new TransportConfig { port = 18090, portRange = 50, randomizePort = true });

                    _logger.LogInformation("Transport created successfully on port {Port}", 5060);
                });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transport");
                throw new TransportException(
                    "Failed to create UDP transport", 
                    "UDP", 
                    5060, 
                    ex);
            }
        }

        private void RegisterAccountInternal(SipAccount account)
        {
            _endpoint.ExecuteSafely(() =>
            {
                if (!_circuitBreaker.CanExecute())
                {
                    throw new SipRegistrationException(
                        "Registration service is currently unavailable",
                        account.AccountId,
                        503); // Service Unavailable
                }

                var acfg = new AccountConfig
                {
                    idUri = $"sip:{account.Username}@{account.Domain}",
                    regConfig = { registrarUri = account.RegistrarUri }
                };
                acfg.mediaConfig.transportConfig.portRange = 500;
                acfg.mediaConfig.transportConfig.port = 0;


                acfg.sipConfig.authCreds.Add(new AuthCredInfo(
                    "digest", "*", account.Username, 0, account.Password));

                var pjsipAccount = new Sip.Account(_context, account.Id, _loggerFactory);
                pjsipAccount.create(acfg);
                
                _accounts[account.AccountId] = pjsipAccount;
                account.IsActive = true;

                try
                {
                    _context.Update(account);
                    _context.SaveChanges();
                    _circuitBreaker.OnSuccess();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update account status");
                    throw;
                }
            });
        }

        public void EnqueueTask(Action task) => _taskQueue.Add(task);

        private async Task ProcessTaskQueue()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var task = _taskQueue.Take(_cts.Token);
                    await Task.Run(() =>
                    {
                        _endpoint.ExecuteSafely(() => task());
                    }, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Task processing failed");
                }
            }
        }

        public async Task<SipAccount> RegisterAccountAsync(SipAccount account)
        {
            var tcs = new TaskCompletionSource<SipAccount>();
            
            EnqueueTask(() =>
            {
                try
                {
                    _endpoint.ExecuteSafely(() =>
                    {
                        RegisterAccountInternal(account);
                        tcs.SetResult(account);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Account registration failed due to thread registration or other error");
                    tcs.SetException(ex);
                }
            });

            try 
            {
                return await tcs.Task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete account registration task");
                throw;
            }
        }

        public void MakeCall(string accountId, string destUri)
        {
            EnqueueTask(() =>
            {
                try
                {
                    _endpoint.ExecuteSafely(() =>
                    {
                        if (!_accounts.TryGetValue(accountId, out var account))
                        {
                            throw new SipCallException(
                                "Account not found",
                                -1,
                                "INVALID_ACCOUNT");
                        }

                        var call = new Sip.Call(account, _context, _loggerFactory);
                        var prm = new CallOpParam(true);
                        call.makeCall(destUri, prm);
                        
                        var sipCall = new SipCall
                        {
                            CallId = call.getId(),
                            RemoteUri = destUri,
                            Status = "INITIATED",
                            SipAccountId = account.DbId
                        };
                        
                        try
                        {
                            _context.SipCalls.Add(sipCall);
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to save call record");
                            throw;
                        }
                        
                        _activeCalls.TryAdd(call.getId(), (call, accountId));
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to make call to {DestUri}", destUri);
                    throw new SipCallException(
                        "Failed to initiate call",
                        -1,
                        "CALL_FAILED",
                        ex);
                }
            });
        }

        public void HangupCall(int callId)
        {
            EnqueueTask(() =>
            {
                _endpoint.ExecuteSafely(() =>
                {
                    if (!_activeCalls.TryRemove(callId, out var callInfo))
                    {
                        _logger.LogWarning("Attempted to hang up non-existent call {CallId}", callId);
                        return;
                    }

                    try
                    {
                        callInfo.Call.hangup(new CallOpParam());
                        
                        try
                        {
                            var dbCall = _context.SipCalls.First(c => c.CallId == callId);
                            dbCall.EndedAt = DateTime.UtcNow;
                            dbCall.Status = "TERMINATED";
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to update call record");
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error hanging up call {CallId}", callId);
                        throw new SipCallException(
                            "Failed to hang up call",
                            callId,
                            "HANGUP_FAILED",
                            ex);
                    }
                });
            });
        }

        public void Dispose()
        {
            try
            {
                _cts.Cancel();
                _endpoint.Instance.libDestroy();
                _taskQueue.CompleteAdding();
                _workerTask.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during disposal");
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
