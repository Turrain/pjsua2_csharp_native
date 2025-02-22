using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PjSip.App.Models;

namespace PjSip.App.Hubs
{
    public class SipHub(ILogger<SipHub> logger) : Hub
    {
        private readonly ILogger<SipHub> _logger = logger;

        public Task Ping()
        {
            _logger.LogInformation("Ping received from client {ConnectionId}", Context.ConnectionId);
            return Clients.Caller.SendAsync("Pong", "Server responded at " + DateTime.UtcNow);
        }

        public async Task SendCallUpdate(SipCall call)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(call);
                _logger.LogInformation("Sending CallUpdate for call {CallId}", call.CallId);
                await Clients.All.SendAsync("CallUpdate", call);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending CallUpdate");
                throw; // Re-throw to let the client know of the failure
            }
        }

        public async Task SendAccountUpdate(SipAccount account)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(account);
                _logger.LogInformation("Sending AccountUpdate for account {AccountId}", account.AccountId);
                await Clients.All.SendAsync("AccountUpdate", account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending AccountUpdate");
                throw;
            }
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}. Reason: {Exception}", 
                Context.ConnectionId, exception?.Message ?? "Unknown");
            return base.OnDisconnectedAsync(exception);
        }
    }
}