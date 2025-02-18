using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PjSua2.Native.pjsua2;
using PjSip.App.Exceptions;
using PjSip.App.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using PjSip.App.Utils;
using PjSip.App.Models;

namespace PjSip.App.Services
{
    public class SipManager
    {
        private readonly BlockingCollection<ISipCommand> _commandQueue = new();
        private readonly ILogger<SipManager> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SipOperationPolicies _policies;
        private readonly ConcurrentDictionary<string, Sip.Account> _accounts = new();
        private readonly ConcurrentDictionary<int, (Sip.Call Call, string AccountId)> _activeCalls = new();

        public SipManager(
            ILogger<SipManager> logger,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _policies = new SipOperationPolicies(logger);

            InitializePjsip();
            StartCommandProcessor();
        }

        private void InitializePjsip()
        {
            _policies.ExecuteSipOperation(() =>
            {
                var transportConfig = new TransportConfig
                {
                    port = 18090,
                    portRange = 50,
                    randomizePort = true
                };

                ThreadSafeEndpoint.Instance.ExecuteSafely(() =>
                {
                    ThreadSafeEndpoint.Instance.InstanceEndpoint.transportCreate(
                        pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                        transportConfig);

                    ThreadSafeEndpoint.Instance.InstanceEndpoint.audDevManager().setNullDev();
                });

                return true;
            }, "PJSIP Initialization");
        }

        private void StartCommandProcessor() =>
            Task.Run(ProcessCommandsAsync);

        private async Task ProcessCommandsAsync()
        {
            foreach (var command in _commandQueue.GetConsumingEnumerable())
            {
                await _policies.ExecuteWithRetry(async () =>
                {
                    await Task.Run(() =>
                        ThreadSafeEndpoint.Instance.ExecuteSafely(command.Execute));
                });
            }
        }

        public Task RegisterAccountAsync(SipAccount account) =>
            ExecuteCommand(new RegisterAccountCommand(
                account, _serviceScopeFactory, _loggerFactory, _accounts));

        public Task MakeCallAsync(string accountId, string destUri) =>
            ExecuteCommand(new MakeCallCommand(
                accountId, destUri, _serviceScopeFactory, _activeCalls, _accounts, _loggerFactory));

        public Task HangupCallAsync(int callId) =>
            ExecuteCommand(new HangupCallCommand(callId, _serviceScopeFactory, _activeCalls));
        public Task ClearAccountsAsync() =>
    ExecuteCommand(new ClearAccountsCommand(_serviceScopeFactory, _accounts, _loggerFactory));

        private Task ExecuteCommand(ISipCommand command)
        {
            _commandQueue.Add(command);
            return command.CompletionTask;
        }

        #region Policy Classes
        private class SipOperationPolicies
        {
            private readonly CircuitBreaker _circuitBreaker;
            private readonly ILogger _logger;

            public SipOperationPolicies(ILogger logger)
            {
                _logger = logger;
                _circuitBreaker = new CircuitBreaker(
                    failureThreshold: 5,
                    resetTimeout: TimeSpan.FromMinutes(1));
            }

            public T ExecuteSipOperation<T>(Func<T> operation, string operationName)
            {
                try
                {
                    if (!_circuitBreaker.CanExecute())
                        throw new SipServiceUnavailableException();

                    var result = operation();
                    _circuitBreaker.OnSuccess();
                    return result;
                }
                catch (Exception ex)
                {
                    _circuitBreaker.OnFailure();
                    _logger.LogError(ex, "{Operation} failed", operationName);
                    throw;
                }
            }

            public async Task ExecuteWithRetry(Func<Task> operation)
            {
                const int maxRetries = 3;
                for (var attempt = 0; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        await operation();
                        return;
                    }
                    catch (Exception ex) when (attempt < maxRetries)
                    {
                        _logger.LogWarning(ex, "Retry attempt {Attempt}/3", attempt + 1);
                        await Task.Delay(1000 * (attempt + 1));
                    }
                }
            }
        }

        private class CircuitBreaker
        {
            private readonly int _failureThreshold;
            private readonly TimeSpan _resetTimeout;
            private int _failureCount;
            private DateTime _lastFailureTime = DateTime.MinValue;

            public CircuitBreaker(int failureThreshold, TimeSpan resetTimeout)
            {
                _failureThreshold = failureThreshold;
                _resetTimeout = resetTimeout;
            }

            public bool CanExecute()
            {
                if (_failureCount > _failureThreshold &&
                   (DateTime.UtcNow - _lastFailureTime) < _resetTimeout)
                {
                    return false;
                }
                return true;
            }

            public void OnSuccess() => Reset();

            public void OnFailure()
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;
            }

            private void Reset()
            {
                _failureCount = 0;
                _lastFailureTime = DateTime.MinValue;
            }
        }
        #endregion

        #region Command Implementations
        private interface ISipCommand
        {
            void Execute();
            Task CompletionTask { get; }
        }

        private abstract class SipCommandBase : ISipCommand
        {
            private readonly TaskCompletionSource<bool> _tcs = new();
            public Task CompletionTask => _tcs.Task;

            public void Execute()
            {
                try
                {
                    ExecuteCore();
                    _tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    _tcs.SetException(ex);
                }
            }

            protected abstract void ExecuteCore();
        }

        private class RegisterAccountCommand : SipCommandBase
        {
            private readonly SipAccount _account;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ILoggerFactory _loggerFactory;
            private readonly ConcurrentDictionary<string, Sip.Account> _accounts;

            public RegisterAccountCommand(
                SipAccount account,
                IServiceScopeFactory scopeFactory,
                ILoggerFactory loggerFactory,
                ConcurrentDictionary<string, Sip.Account> accounts)
            {
                _account = account;
                _scopeFactory = scopeFactory;
                _loggerFactory = loggerFactory;
                _accounts = accounts;
            }

            protected override void ExecuteCore()
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                // Update account status
                _account.IsActive = false;
                context.Update(_account);
                context.SaveChanges();

                // Configure SIP account
                var acfg = new AccountConfig
                {
                    idUri = $"sip:{_account.Username}@{_account.Domain}",
                    regConfig = { registrarUri = _account.RegistrarUri },
                    mediaConfig = { transportConfig = { portRange = 500, port = 0 } }
                };

                acfg.sipConfig.authCreds.Add(new AuthCredInfo(
                    "digest", "*", _account.Username, 0, _account.Password));

                // Create and register account
                var pjsipAccount = new Sip.Account(
                    context,
                    _account.Id,
                    _loggerFactory,
                    _scopeFactory);

                pjsipAccount.create(acfg);
                _accounts[_account.AccountId] = pjsipAccount;
            }
        }
        private class ClearAccountsCommand : SipCommandBase
        {
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ConcurrentDictionary<string, Sip.Account> _accounts;
            private readonly ILogger _logger;

            public ClearAccountsCommand(
                IServiceScopeFactory scopeFactory,
                ConcurrentDictionary<string, Sip.Account> accounts,
                ILoggerFactory loggerFactory)
            {
                _scopeFactory = scopeFactory;
                _accounts = accounts;
                _logger = loggerFactory.CreateLogger<ClearAccountsCommand>();
            }

            protected override void ExecuteCore()
            {
                // Log the clear operation.
                _logger.LogInformation("Clearing all SIP accounts.");

                // Optionally, update database entries here if needed.
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                // Example: Mark all accounts as inactive in the database.
                var dbAccounts = context.SipAccounts.ToList();
                foreach (var account in dbAccounts)
                {
                    account.IsActive = false;
                    context.Update(account);
                }
                context.SaveChanges();

                // Clear the in-memory accounts.
                _accounts.Clear();
            }
        }
        private class MakeCallCommand : SipCommandBase
        {
            private readonly string _accountId;
            private readonly string _destUri;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ILoggerFactory _loggerFactory;
            private readonly ConcurrentDictionary<int, (Sip.Call Call, string AccountId)> _activeCalls;
            private readonly ConcurrentDictionary<string, Sip.Account> _accounts;

            public MakeCallCommand(
                string accountId,
                string destUri,
                IServiceScopeFactory scopeFactory,
                ConcurrentDictionary<int, (Sip.Call, string)> activeCalls,
                ConcurrentDictionary<string, Sip.Account> accounts,
                ILoggerFactory loggerFactory)
            {
                _accountId = accountId;
                _destUri = destUri;
                _scopeFactory = scopeFactory;
                _activeCalls = activeCalls;
                _accounts = accounts;
                _loggerFactory = loggerFactory;
            }

            protected override void ExecuteCore()
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                if (!_accounts.TryGetValue(_accountId, out var account))
                    throw new SipCallException("Account not found", -1, "INVALID_ACCOUNT");

                 var call = new Sip.Call(account, -1, _loggerFactory, _scopeFactory);
                var prm = new CallOpParam(true);
                call.makeCall(_destUri, prm);

                var dbAccount = context.SipAccounts.Find(account.DbId) ??
                    throw new InvalidOperationException($"Account {_accountId} not found");

                var sipCall = new SipCall(
                    callId: call.getId(),
                    remoteUri: _destUri,
                    status: "INITIATED",
                    account: dbAccount
                );

                context.SipCalls.Add(sipCall);
                context.SaveChanges();

                _activeCalls.TryAdd(call.getId(), (call, _accountId));
            }
        }

        private class HangupCallCommand : SipCommandBase
        {
            private readonly int _callId;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ConcurrentDictionary<int, (Sip.Call Call, string AccountId)> _activeCalls;
            private readonly ILogger _logger;

            public HangupCallCommand(
                int callId,
                IServiceScopeFactory scopeFactory,
                ConcurrentDictionary<int, (Sip.Call, string)> activeCalls)
            {
                _callId = callId;
                _scopeFactory = scopeFactory;
                _activeCalls = activeCalls;
                _logger = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<HangupCallCommand>>();
            }

            protected override void ExecuteCore()
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                if (!_activeCalls.TryRemove(_callId, out var callInfo))
                {
                    _logger.LogWarning("Call {CallId} not found", _callId);
                    return;
                }

                callInfo.Call.hangup(new CallOpParam());

                var dbCall = context.SipCalls.FirstOrDefault(c => c.CallId == _callId);
                if (dbCall != null)
                {
                    dbCall.EndedAt = DateTime.UtcNow;
                    dbCall.Status = "TERMINATED";
                    context.SaveChanges();
                }
            }
        }
        #endregion
    }

    public class SipServiceUnavailableException : Exception
    {
        public SipServiceUnavailableException()
            : base("SIP service is currently unavailable") { }
    }
}