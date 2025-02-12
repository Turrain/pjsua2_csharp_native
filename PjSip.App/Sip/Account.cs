using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PjSua2.Native.pjsua2;
using PjSip.App.Services;
using PjSip.App.Exceptions;
using PjSip.App.Data;

namespace PjSip.App.Sip
{
    public class Account : PjSua2.Native.pjsua2.Account
    {
        private readonly SipDbContext _context;
        private readonly ILogger<Account> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public int DbId { get; }

        public Account(SipDbContext context, int dbId, ILoggerFactory loggerFactory)
        {
            _context = context;
            DbId = dbId;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Account>();
        }

        public override void onIncomingCall(OnIncomingCallParam prm)
        {
            try
            {
                _logger.LogInformation("Incoming call received for account {DbId}", DbId);

                var call = new Call(this, _context, _loggerFactory)
                {
                    CallDirection = Call.Direction.Incoming
                };

                try
                {
                    _context.SipCalls.Add(new SipCall
                    {
                        CallId = prm.callId,
                        Status = "INCOMING",
                        SipAccountId = DbId,
                        StartedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save incoming call record");
                    throw;
                }

                try
                {
                    var callOpParam = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK };
                    call.answer(callOpParam);
                    _logger.LogInformation("Call {CallId} answered successfully", prm.callId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to answer call {CallId}", prm.callId);
                    throw new SipCallException(
                        "Failed to answer incoming call",
                        prm.callId,
                        "ANSWER_FAILED",
                        ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling incoming call for account {DbId}", DbId);
            }
        }

        public override void onRegState(OnRegStateParam prm)
        {
            try
            {
                var isRegistered = prm.code == pjsip_status_code.PJSIP_SC_OK;
                _logger.LogInformation(
                    "Registration state changed for account {DbId}. Status: {Status}", 
                    DbId, 
                    isRegistered ? "Registered" : "Not Registered");

                Task.Run(async () =>
                {
                    try
                    {
                        var account = await _context.SipAccounts.FindAsync(DbId);
                        if (account != null)
                        {
                            account.IsActive = isRegistered;
                            await _context.SaveChangesAsync();

                            _logger.LogInformation(
                                "Account {DbId} status updated to {Status}", 
                                DbId, 
                                isRegistered);
                        }
                        else
                        {
                            _logger.LogWarning("Account {DbId} not found in database", DbId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to update account status for {DbId}", DbId);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling registration state change for account {DbId}", DbId);
            }
        }
    }
}
