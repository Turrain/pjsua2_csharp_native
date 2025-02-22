using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PjSip.App.Data;
using PjSip.App.Exceptions;
using PjSip.App.Models;

namespace PjSip.App.Services
{
    public class SipManagerService
    {
        private readonly SipManager _sipManager;
        private readonly ILogger<SipManagerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SipManagerService(
            SipManager sipManager,
            ILogger<SipManagerService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _sipManager = sipManager;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        public async Task<SipAccount> UpdateAccountAgentAsync(string accountId, int agentConfigId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

            _logger.LogInformation($"Updating agent for account {accountId} to agent {agentConfigId}");

            var account = await context.SipAccounts
                .Include(a => a.Agent)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                throw new SipRegistrationException("Account not found", accountId, 404);

            var agentConfig = await context.AgentConfigs.FindAsync(agentConfigId);
            if (agentConfig == null)
                throw new ArgumentException("Agent configuration not found", nameof(agentConfigId));

            account.Agent = agentConfig;
            await context.SaveChangesAsync();



            return account;
        }

        public async Task<SipAccount> RegisterAccountAsync(SipAccount account)
        {
            try
            {
                ValidateAccount(account);
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                var existingAccount = await context.SipAccounts
                    .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

                if (existingAccount != null)
                {
                    throw new SipRegistrationException(
                        "Account already exists",
                        account.AccountId,
                        409);
                }

                await _sipManager.RegisterAccountAsync(account);
                return await context.SipAccounts
                    .FirstAsync(a => a.AccountId == account.AccountId);
            }
            catch (SipRegistrationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Account registration failed");
                throw new SipRegistrationException(
                    "Registration failed",
                    account.AccountId,
                    500,
                    ex);
            }
        }

        public async Task ClearAccountsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

            await ClearDatabaseEntities(context);
            await _sipManager.ClearAccountsAsync();
        }

        public async Task<SipCall> MakeCallAsync(string accountId, string destination)
        {
            try
            {
                ValidateCallParameters(accountId, destination);

                await _sipManager.MakeCallAsync(accountId, destination);

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                return await context.SipCalls
                    .OrderByDescending(c => c.StartedAt)
                    .FirstAsync(c => c.Account.AccountId == accountId &&
                                    c.RemoteUri == destination);
            }
            catch (SipCallException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Call initiation failed");
                throw new SipCallException(
                    "Call failed",
                    -1,
                    "CALL_FAILURE",
                    ex);
            }
        }

        public async Task<SipCall> GetCallStatusAsync(int callId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

            var call = await context.SipCalls
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CallId == callId);

            return call ?? throw new SipCallException(
                "Call not found",
                callId,
                "NOT_FOUND");
        }

        public async Task HangupCallAsync(int callId)
        {
            try
            {
                await _sipManager.HangupCallAsync(callId);

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                var call = await context.SipCalls.FindAsync(callId);
                if (call != null)
                {
                    call.Status = "TERMINATED";
                    call.EndedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Call termination failed");
                throw new SipCallException(
                    "Hangup failed",
                    callId,
                    "HANGUP_FAILURE",
                    ex);
            }
        }

        public async Task<IEnumerable<SipAccount>> GetAllAccountsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

            return await context.SipAccounts.Include(a => a.Agent) .Include(a => a.Calls) .AsNoTracking().ToListAsync();
        }
    
        private static async Task ClearDatabaseEntities(SipDbContext context)
        {
            context.SipCalls.RemoveRange(await context.SipCalls.ToListAsync());
            context.SipAccounts.RemoveRange(await context.SipAccounts.ToListAsync());
            await context.SaveChangesAsync();
        }

        private static void ValidateAccount(SipAccount account)
        {
            if (string.IsNullOrEmpty(account.Username))
                throw new ArgumentException("Username required");

            if (string.IsNullOrEmpty(account.Password))
                throw new ArgumentException("Password required");

            if (string.IsNullOrEmpty(account.Domain))
                throw new ArgumentException("Domain required");
        }

        private static void ValidateCallParameters(string accountId, string destination)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentException("Account ID required");

            if (string.IsNullOrEmpty(destination))
                throw new ArgumentException("Destination required");
        }
    }
}