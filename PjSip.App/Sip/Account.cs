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
using Microsoft.AspNetCore.SignalR;
using PjSip.App.Hubs;

namespace PjSip.App.Sip
{
    public class Account : PjSua2.Native.pjsua2.Account
    {
          private readonly IHubContext<SipHub> _hubContext;
        private readonly ILogger<Account> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _dbId;
        public AgentConfig Agent { get; set; }
        private readonly MediaPortManager _mediaPortManager;

        public int DbId => _dbId;

       public Account(SipDbContext context, int dbId, ILoggerFactory loggerFactory, 
    IServiceScopeFactory serviceScopeFactory, MediaPortManager mediaPortManager, IHubContext<SipHub> hubContext)
{
    _dbId = dbId;
    _loggerFactory = loggerFactory;
    _logger = loggerFactory.CreateLogger<Account>();
    _serviceScopeFactory = serviceScopeFactory;
    _hubContext = hubContext;
    _mediaPortManager = mediaPortManager ?? throw new ArgumentNullException(nameof(mediaPortManager));
}

private async Task NotifyAccountChange(SipAccount account)
{
    try
    {
        if (account == null)
        {
            _logger.LogWarning("Cannot notify about null account for DbId {DbId}", _dbId);
            return;
        }

        if (_hubContext == null)
        {
            _logger.LogWarning("HubContext is null for account {DbId}", _dbId);
            return;
        }

        // Include related data before sending notification
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
        
        var accountWithDetails = await context.SipAccounts
            .Include(a => a.Agent)
            .Include(a => a.Calls)
            .FirstOrDefaultAsync(a => a.Id == _dbId);

        if (accountWithDetails == null)
        {
            _logger.LogWarning("Account {DbId} not found in database", _dbId);
            return;
        }
        Console.WriteLine("Account with details: " + accountWithDetails);
        await _hubContext.Clients.All.SendAsync("AccountUpdate", accountWithDetails);
        _logger.LogDebug("Account update notification sent for account {DbId}", _dbId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send account update notification for account {DbId}", _dbId);
    }
}

        public override void onIncomingCall(OnIncomingCallParam prm)
        {
            try
            {
                _logger.LogInformation("Incoming call received for account {DbId}", _dbId);

                var call = new Call(this, prm.callId, _loggerFactory, _serviceScopeFactory, _mediaPortManager, _hubContext)
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
                NotifyAccountChange(account);
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
        _logger.LogInformation("Registration state changed for account {DbId}: {Status}", _dbId, isRegistered);

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();

        // Update database synchronously to ensure it completes
        var account = context.SipAccounts
            .Include(a => a.Agent)
            .Include(a => a.Calls)
            .FirstOrDefault(a => a.Id == _dbId);

        if (account != null)
        {
            account.IsActive = isRegistered;
            context.SaveChanges();
            
            // Send notification synchronously
           NotifyAccountChange(account).Wait();
                
            _logger.LogDebug("Account {DbId} status updated and notification sent", _dbId);
        }
        else
        {
            _logger.LogWarning("Account {DbId} not found in database", _dbId);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error handling registration state change for account {DbId}", _dbId);
    }
}
    }
}