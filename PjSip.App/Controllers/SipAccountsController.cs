using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PjSip.App.Services;
using PjSip.App.Exceptions;

namespace PjSip.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SipAccountsController : ControllerBase
    {
        private readonly SipManagerService _sipManager;
        private readonly ILogger<SipAccountsController> _logger;

        public SipAccountsController(
            SipManagerService sipManager, 
            ILogger<SipAccountsController> logger)
        {
            _sipManager = sipManager;
            _logger = logger;
        }

        public class RegisterAccountRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Domain { get; set; }
            public string RegistrarUri { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterAccountRequest request)
        {
            try
            {
                var account = new SipAccount
                {
                    AccountId = Guid.NewGuid().ToString(),
                    Username = request.Username,
                    Password = request.Password,
                    Domain = request.Domain,
                    RegistrarUri = request.RegistrarUri,
                    CreatedAt = DateTime.UtcNow
                };

                var registeredAccount = await _sipManager.RegisterAccountAsync(account);
                return Ok(registeredAccount);
            }
            catch (SipRegistrationException ex)
            {
                _logger.LogError(ex, "Failed to register account");
                return StatusCode(500, new { error = ex.Message, code = ex.ErrorCode });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid account registration request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during account registration");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }
    }
}
