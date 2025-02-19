using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PjSip.App.Services;
using PjSip.App.Exceptions;
using PjSip.App.Models;
using static PjSip.App.Models.AgentConfig;

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
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                var accounts = await _sipManager.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Error retrieving accounts. CorrelationId: {CorrelationId}",
                    correlationId);
                return StatusCode(500, new
                {
                    error = "Failed to retrieve accounts",
                    correlationId = correlationId
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> ClearAccounts()
        {
            try
            {
                await _sipManager.ClearAccountsAsync();
                return Ok(new { message = "All accounts have been cleared" });
            }
            catch (SipRegistrationException ex)
            {
                _logger.LogError(ex, "Failed to clear accounts");
                return StatusCode(500, new { error = ex.Message, code = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Unexpected error clearing accounts. CorrelationId: {CorrelationId}",
                    correlationId);
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred while clearing accounts",
                    correlationId = correlationId
                });
            }
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
                    CreatedAt = DateTime.UtcNow,
                    Agent = new AgentConfig
                    {
                        AgentId = Guid.NewGuid().ToString(),
                        LLM = new LLMConfig
                        {
                            Model = "gpt-3.5-turbo", // Required
                            OllamaEndpoint = "http://localhost:11434" // Required
                        },
                        Whisper = new WhisperConfig
                        {
                            Endpoint = "http://localhost:9000" // Required
                        },
                        Auralis = new AuralisConfig
                        {
                            Endpoint = "http://localhost:8000", // Required
                            ApiKey = "your-api-key-here" // Required
                        }
                    }
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
