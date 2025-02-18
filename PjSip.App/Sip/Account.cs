using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PjSua2.Native.pjsua2;
using PjSip.App.Services;
using PjSip.App.Exceptions;
using PjSip.App.Data;
using Microsoft.Extensions.DependencyInjection;
using PjSip.App.Models;

namespace PjSip.App.Sip
{
    public class Account : PjSua2.Native.pjsua2.Account
    {
        private readonly ILogger<Account> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _dbId;

        public int DbId => _dbId;

        public Account(SipDbContext context, int dbId, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            // No longer storing the injected context to avoid disposal issues.
            _dbId = dbId;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Account>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override void onIncomingCall(OnIncomingCallParam prm)
        {
            try
            {
                _logger.LogInformation("Incoming call received for account {DbId}", _dbId);

                var call = new Call(this, prm.callId, _loggerFactory, _serviceScopeFactory)
                {
                    CallDirection = Call.Direction.Incoming
                };

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                var account = dbContext.SipAccounts.Find(_dbId);

                dbContext.SipCalls.Add(new SipCall(
                    callId: prm.callId,
                    remoteUri: prm.rdata.info,  // Assuming this holds the remote URI
                    status: "INCOMING",
                    account: account
                ));

                dbContext.SaveChanges();

                try
                {
                    var callOpParam = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK };
                    call.answer(callOpParam);
                    _logger.LogInformation("Call {CallId} answered successfully", prm.callId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to answer call {CallId}", prm.callId);
                    throw new SipCallException("Failed to answer incoming call", prm.callId, "ANSWER_FAILED", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling incoming call for account {DbId}", _dbId);
            }
        }

        public override void onRegState(OnRegStateParam prm)
        {
            try
            {
                var isRegistered = prm.code == pjsip_status_code.PJSIP_SC_OK;

                Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

                    // Add retry logic.
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            var account = await context.SipAccounts.FindAsync(_dbId);
                            if (account != null)
                            {
                                account.IsActive = isRegistered;
                                await context.SaveChangesAsync();
                                _logger.LogInformation("Account {DbId} status updated to {Status}", _dbId, isRegistered);
                                return;
                            }
                            await Task.Delay(500); // Wait before retry.
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to update account status for {DbId}", _dbId);
                            if (i == 2) throw;
                        }
                    }
                    _logger.LogError("Account {DbId} not found after retries", _dbId);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling registration state change for account {DbId}", _dbId);
            }
        }
    }
}