
using System.Collections.Concurrent;
using PjSua2.Native.pjsua2;

namespace PjSua2.Lx
{
    public struct TaskStatus
    {
        public bool Success;
        public string Message;
        public int StatusCode;
    }

    /// <summary>
    /// A simple task queue based on BlockingCollection to serialize SIP operations.
    /// </summary>
    public class TaskQueue
    {
        private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();

        public void Enqueue(Action task)
        {
            _queue.Add(task);
        }

        public Action Dequeue(CancellationToken token)
        {
            return _queue.Take(token);
        }

        public void Stop()
        {
            _queue.CompleteAdding();
        }
    }

    /// <summary>
    /// Manager class that wraps PJSUA2 operations such as account registration, call control, and shutdown.
    /// All operations are dispatched onto a dedicated worker thread.
    /// </summary>
    public class Manager : IDisposable
    {
        private Endpoint _endpoint;
        private readonly Dictionary<string, Account> _accounts = new Dictionary<string, Account>();

        // Dictionary mapping call IDs to Call objects.
        private readonly Dictionary<int, Call> _activeCalls = new Dictionary<int, Call>();

        // New dictionary: map each call ID to the accountId that initiated it.
        private readonly Dictionary<int, string> _callToAccountMap = new Dictionary<int, string>();

        private readonly TaskQueue _taskQueue = new TaskQueue();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _workerTask;
        private readonly object _accountsLock = new object();
        private readonly object _callsLock = new object();
        private volatile bool _running = true;

        public Manager()
        {
            // Initialize the PJSUA2 endpoint.
            _endpoint = new Endpoint();
            _endpoint.libCreate();

            EpConfig epConfig = new EpConfig();
            epConfig.logConfig.level = 1; // Set desired log level.

            _endpoint.libInit(epConfig);
       
            // Create a UDP transport on port 18090.
            TransportConfig tcfg = new TransportConfig();
            tcfg.port = 18090;
            tcfg.portRange = 50;
            tcfg.randomizePort = true;

            _endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, tcfg);

            // Disable the audio device.
            _endpoint.audDevManager().setNullDev();

            // Start the PJSUA2 library.
            _endpoint.libStart();
            Console.WriteLine("PJSUA2 library started.");

            // Start the background worker thread to process queued tasks.
            _workerTask = Task.Run(() => WorkerThreadMain(_cts.Token));
        }

        /// <summary>
        /// Main loop for processing tasks.
        /// </summary>
        private void WorkerThreadMain(CancellationToken token)
        {
            try
            {
                // Register this external thread.
                _endpoint.libRegisterThread("WorkerThread");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to register worker thread: " + ex.Message);
                return;
            }
            try
            {
                while (_running)
                {
                    Action task = _taskQueue.Dequeue(token);
                    if (task != null)
                    {
                        try
                        {
                            task();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error executing task: " + ex.Message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown.
            }

            // When the worker exits, perform final PJSUA2 shutdown.
            ShutdownPjsip();
        }

        /// <summary>
        /// Adds a new SIP account and initiates registration.
        /// Returns a Task that completes with a TaskStatus.
        /// </summary>
        public Task<TaskStatus> AddAccount(
            string accountId,
            string domain,
            string username,
            string password,
            string registrarUri,
            string agentId = "")
        {
            var tcs = new TaskCompletionSource<TaskStatus>();

            // Enqueue the registration task.
            _taskQueue.Enqueue(() =>
            {
                try
                {
                    lock (_accountsLock)
                    {
                        if (_accounts.ContainsKey(accountId))
                        {
                            throw new Exception("Account already exists: " + accountId);
                        }

                        // Prepare the account configuration.
                        AccountConfig acfg = new()
                        {
                            idUri = $"sip:{username}@{domain}"
                        };

                        acfg.regConfig.registrarUri = registrarUri;
                        acfg.regConfig.timeoutSec = 20;
                        acfg.regConfig.retryIntervalSec = 2;

                        // Set up authentication credentials.
                        AuthCredInfo cred = new("digest", "*", username, 0, password);
                        acfg.sipConfig.authCreds.Add(cred);
                        //FIXED: Only on C# version
                        acfg.mediaConfig.transportConfig.portRange = 500;
                        acfg.mediaConfig.transportConfig.port = 0;
                        // NAT settings.
                        acfg.natConfig.sipStunUse = pjsua_stun_use.PJSUA_STUN_USE_DEFAULT;
                        acfg.natConfig.mediaStunUse = pjsua_stun_use.PJSUA_STUN_USE_DEFAULT;
                        acfg.natConfig.contactRewriteUse = 1;

                        // Create our account.
                        var account = new Account(tcs);
                        account.create(acfg);
                        if (!string.IsNullOrEmpty(agentId))
                        {
                            account.AgentId = agentId;
                        }

                        _accounts.Add(accountId, account);
                    }
                }
                catch (Exception ex)
                {
                    if (!tcs.Task.IsCompleted)
                        tcs.SetResult(new TaskStatus
                        {
                            Success = false,
                            Message = "Error: " + ex.Message,
                            StatusCode = 500
                        });
                }
            });

            // Apply a timeout of 20 seconds on registration.
            Task delayTask = Task.Delay(TimeSpan.FromSeconds(20));
            return Task.WhenAny(tcs.Task, delayTask).ContinueWith(task =>
            {
                if (task.Result == delayTask)
                {
                    return new TaskStatus
                    {
                        Success = false,
                        Message = "Registration timeout",
                        StatusCode = 408
                    };
                }
                return tcs.Task.Result;
            });
        }

        /// <summary>
        /// Removes an account by shutting it down and erasing it from our dictionary.
        /// </summary>
        public void RemoveAccount(string accountId)
        {
            _taskQueue.Enqueue(() =>
            {
                try
                {
                    lock (_accountsLock)
                    {
                        if (_accounts.TryGetValue(accountId, out Account account))
                        {
                            account.shutdown();
                            _accounts.Remove(accountId);
                            Console.WriteLine("Account removed: " + accountId);
                        }
                        else
                        {
                            Console.WriteLine("Account not found: " + accountId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error removing account: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Makes a call from the specified account to the destination URI.
        /// In addition to creating the call, we now also record the originating account.
        /// </summary>
        public void MakeCall(string accountId, string destUri)
        {
            _taskQueue.Enqueue(() =>
            {
                try
                {
                    Account account;
                    lock (_accountsLock)
                    {
                        if (!_accounts.TryGetValue(accountId, out account))
                        {
                            throw new Exception("Account not found: " + accountId);
                        }
                    }

                    // Create and start the call.
                    var call = new Call(account);
                    CallOpParam callOpParam = new CallOpParam(true);
                    call.makeCall(destUri, callOpParam);

                    int callId = call.getId();
                    lock (_callsLock)
                    {
                        _activeCalls.Add(callId, call);
                        _callToAccountMap.Add(callId, accountId);
                    }
                    Console.WriteLine($"Call {callId} created for account {accountId} calling {destUri}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error making call: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Hangs up an active call by call ID.
        /// Also removes the call from the call-to-account mapping.
        /// </summary>
     public void HangupCall(int callId)
{
    _taskQueue.Enqueue(() =>
    {
        lock (_callsLock)
        {
            if (_activeCalls.TryGetValue(callId, out Call call))
            {
                try
                {
                    // Try to retrieve call state.
                    pjsip_inv_state state;
                    try
                    {
                        var info = call.getInfo();
                        state = info.state;
                    }
                    catch (Exception ex)
                    {
                        // If the error indicates the session is already terminated,
                        // we consider the call as already finished.
                        if (ex.Message.Contains("INVITE session already terminated"))
                        {
                            Console.WriteLine($"Call {callId} is already terminated (detected during getInfo), cleaning up.");
                            _activeCalls.Remove(callId);
                            _callToAccountMap.Remove(callId);
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    // If the call is already disconnected, just clean up.
                    if (state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                    {
                        Console.WriteLine($"Call {callId} is already disconnected.");
                        _activeCalls.Remove(callId);
                        _callToAccountMap.Remove(callId);
                        return;
                    }

                    // Otherwise, attempt to hang up the call.
                    CallOpParam callOpParam = new CallOpParam();
                    callOpParam.statusCode = pjsip_status_code.PJSIP_SC_DECLINE;
                    call.hangup(callOpParam);
                    _activeCalls.Remove(callId);
                    _callToAccountMap.Remove(callId);
                }
                catch (Exception ex)
                {
                    // If the error is due to the session already being terminated, log and ignore.
                    if (ex.Message.Contains("INVITE session already terminated"))
                    {
                        Console.WriteLine($"Call {callId} is already terminated, ignoring hangup.");
                        _activeCalls.Remove(callId);
                        _callToAccountMap.Remove(callId);
                    }
                    else
                    {
                        Console.WriteLine("Error hanging up call: " + ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Call not found: " + callId);
            }
        }
    });
}

        /// <summary>
        /// Retrieves the account ID associated with a given call ID.
        /// Returns null if the call is not found.
        /// </summary>
        public string GetAccountIdForCall(int callId)
        {
            lock (_callsLock)
            {
                return _callToAccountMap.TryGetValue(callId, out var accountId)
                    ? accountId
                    : null;
            }
        }

        /// <summary>
        /// Gets a list of call IDs that belong to the specified account.
        /// </summary>
        public List<int> GetCallIdsForAccount(string accountId)
        {
            lock (_callsLock)
            {
                return _callToAccountMap
                    .Where(kv => kv.Value == accountId)
                    .Select(kv => kv.Key)
                    .ToList();
            }
        }

        /// <summary>
        /// Hangs up all active calls associated with the specified account.
        /// </summary>
        public void HangupCallsForAccount(string accountId)
        {
            _taskQueue.Enqueue(() =>
            {
                List<int> callsToHangup;
                lock (_callsLock)
                {
                    callsToHangup = _callToAccountMap
                        .Where(kv => kv.Value == accountId)
                        .Select(kv => kv.Key)
                        .ToList();
                }
                foreach (var callId in callsToHangup)
                {
                    HangupCall(callId);
                }
            });
        }

        /// <summary>
        /// Shuts down the Manager, stops processing tasks, and waits for the worker thread to finish.
        /// </summary>
        public void Shutdown()
        {
            _running = false;
            _taskQueue.Stop();
            _cts.Cancel();
            try
            {
                _workerTask.Wait();
            }
            catch (AggregateException)
            {
                // Exceptions during shutdown can be ignored.
            }
        }

        /// <summary>
        /// Shuts down all active calls and accounts, then destroys the PJSUA2 endpoint.
        /// </summary>
        private void ShutdownPjsip()
        {
            // Hangup all active calls.
            lock (_callsLock)
            {
                foreach (var call in _activeCalls.Values)
                {
                    try
                    {
                        CallOpParam callOpParam = new CallOpParam();
                        callOpParam.statusCode = pjsip_status_code.PJSIP_SC_DECLINE;
                        call.hangup(callOpParam);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error hanging up call: " + ex.Message);
                    }
                }
                _activeCalls.Clear();
                _callToAccountMap.Clear();
            }

            // Shutdown all accounts.
            lock (_accountsLock)
            {
                foreach (var account in _accounts.Values)
                {
                    try
                    {
                        account.shutdown();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error shutting down account: " + ex.Message);
                    }
                }
                _accounts.Clear();
            }

            // Destroy the PJSUA2 endpoint.
            _endpoint.libDestroy();
            Console.WriteLine("PJSUA2 library shutdown.");
        }

        public void Dispose()
        {
            Shutdown();
            _cts.Dispose();
        }
    }
}
