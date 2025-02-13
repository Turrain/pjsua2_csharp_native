using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PjSua2.Native.pjsua2;
using PjSip.App.Data;
using PjSip.App.Models;
using PjSip.App.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using PjSip.App.Utils;

namespace PjSip.App.Services
{
    public class SipManagerService : IDisposable
    {
        private readonly SipManager _sipManager;
        private readonly SipDbContext _context;
        private readonly ILogger<SipManagerService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public SipManagerService(
            SipManager sipManager,
            SipDbContext context,
            ILogger<SipManagerService> logger,
            ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            _sipManager = sipManager;
        }

        private void EnsureThreadRegistered()
        {
            try
            {
                // This will throw if thread is not registered
                Endpoint.instance().libIsThreadRegistered();
            }
            catch
            {
                // Register thread with PJSIP if not already registered
                var threadDesc = $"ApiThread-{Thread.CurrentThread.ManagedThreadId}";
                Endpoint.instance().libRegisterThread(threadDesc);
                _logger.LogDebug("Registered API thread {ThreadId} with PJSIP", threadDesc);
            }
        }

        public async Task<SipAccount> RegisterAccountAsync(SipAccount account)
        {
            try
            {
                EnsureThreadRegistered();
                // Validate account data
                if (string.IsNullOrEmpty(account.Username))
                    throw new ArgumentException("Username is required");
                if (string.IsNullOrEmpty(account.Password))
                    throw new ArgumentException("Password is required");
                if (string.IsNullOrEmpty(account.Domain))
                    throw new ArgumentException("Domain is required");
                if (string.IsNullOrEmpty(account.RegistrarUri))
                    throw new ArgumentException("Registrar URI is required");

                return await _sipManager.RegisterAccountAsync(account);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (SipRegistrationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during account registration");
                throw new SipRegistrationException(
                    "Failed to register account due to an unexpected error",
                    account.AccountId,
                    500,
                    ex);
            }
        }
public async Task ClearAccountsAsync()
{
    try
    {
        EnsureThreadRegistered();
        
        // Create TaskCompletionSource to handle the async operation
        var tcs = new TaskCompletionSource();
        
        try
        {
            _sipManager.ClearAccounts();
            tcs.SetResult();
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }

        await tcs.Task;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to clear accounts");
        throw new SipRegistrationException(
            "Failed to clear all accounts",
            "all",
            500,
            ex);
    }
}
        public async Task<SipCall> MakeCallAsync(string accountId, string destination)
        {
            try
            {
                EnsureThreadRegistered();
                // Validate input
                if (string.IsNullOrEmpty(accountId))
                    throw new ArgumentException("Account ID is required");
                if (string.IsNullOrEmpty(destination))
                    throw new ArgumentException("Destination URI is required");

                // Check if account exists and is active
                var account = await _context.SipAccounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive);

                if (account == null)
                    throw new SipCallException("Account not found or inactive", -1, "INVALID_ACCOUNT");

                var call = new SipCall
                {
                    RemoteUri = destination,
                    Status = "INITIATING",
                    SipAccountId = account.Id,
                    StartedAt = DateTime.UtcNow
                };

                try
                {
                    _sipManager.MakeCall(accountId, destination);
                    return call;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initiate call");
                    call.Status = "FAILED";
                    call.EndedAt = DateTime.UtcNow;
                    throw new SipCallException(
                        "Failed to initiate call",
                        call.CallId,
                        "CALL_FAILED",
                        ex);
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (SipCallException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during call initiation");
                throw new SipCallException(
                    "Failed to make call due to an unexpected error",
                    -1,
                    "UNEXPECTED_ERROR",
                    ex);
            }
        }

        public async Task<SipCall> GetCallStatusAsync(int callId)
        {
            try
            {
                EnsureThreadRegistered();
                var call = await _context.SipCalls
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.CallId == callId);

                if (call == null)
                {
                    _logger.LogWarning("Call {CallId} not found", callId);
                    return null;
                }

                return call;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get call status for {CallId}", callId);
                throw new SipCallException(
                    "Failed to retrieve call status",
                    callId,
                    "STATUS_FAILED",
                    ex);
            }
        }

        public async Task HangupCallAsync(int callId)
        {
            try
            {
                EnsureThreadRegistered();
                var call = await _context.SipCalls.FindAsync(callId);
                if (call == null)
                {
                    _logger.LogWarning("Attempted to hang up non-existent call {CallId}", callId);
                    return;
                }

                if (call.EndedAt.HasValue)
                {
                    _logger.LogWarning("Call {CallId} is already ended", callId);
                    return;
                }

                _sipManager.HangupCall(callId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to hang up call {CallId}", callId);
                throw new SipCallException(
                    "Failed to hang up call",
                    callId,
                    "HANGUP_FAILED",
                    ex);
            }
        }
        public async Task<IEnumerable<SipAccount>> GetAllAccountsAsync()
        {

            try
            {
                var accounts = await _context.SipAccounts
                    .Select(a => new SipAccount
                    {
                        Id = a.Id,
                        AccountId = a.AccountId,
                        Username = a.Username,
                        Domain = a.Domain,
                        RegistrarUri = a.RegistrarUri,
                        IsActive = a.IsActive,
                        CreatedAt = a.CreatedAt,
                    })
                    .ToListAsync();

                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve accounts");
                throw;
            }

        }

        public void Dispose()
        {
            try
            {
                EnsureThreadRegistered();
                //      _sipManager?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing SipManager");
            }
        }
    }
}
