using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PjSip.App.Services;
using PjSip.App.Models;
using PjSip.App.Exceptions;

namespace PjSip.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SipCallController : ControllerBase
    {
        private readonly SipManagerService _sipManager;
        private readonly ILogger<SipCallController> _logger;

        public SipCallController(
            SipManagerService sipManager, 
            ILogger<SipCallController> logger)
        {
            _sipManager = sipManager;
            _logger = logger;
        }

        public class CallRequest
        {
            public string AccountId { get; set; }
            public string Destination { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> MakeCall([FromBody] CallRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var call = await _sipManager.MakeCallAsync(request.AccountId, request.Destination);
                return Ok(new
                {
                    callId = call.CallId,
                    status = call.Status,
                    startedAt = call.StartedAt,
                    remoteUri = call.RemoteUri
                });
            }
            catch (SipCallException ex)
            {
                _logger.LogError(ex, "Failed to make call");
                return StatusCode(500, new 
                { 
                    error = ex.Message, 
                    code = ex.CallState,
                    correlationId = ex.CorrelationId
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid call request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Unexpected error making call. CorrelationId: {CorrelationId}", correlationId);
                return StatusCode(500, new 
                { 
                    error = "An unexpected error occurred",
                    correlationId = correlationId
                });
            }
        }

        [HttpPost("{callId}/hangup")]
        public async Task<IActionResult> HangupCall(int callId)
        {
            try
            {
                if (callId <= 0)
                {
                    return BadRequest(new { error = "Invalid call ID" });
                }

                await _sipManager.HangupCallAsync(callId);
                return Ok(new { message = "Call terminated successfully" });
            }
            catch (SipCallException ex)
            {
                _logger.LogError(ex, "Failed to hang up call {CallId}", callId);
                return StatusCode(500, new 
                { 
                    error = ex.Message, 
                    code = ex.CallState,
                    correlationId = ex.CorrelationId
                });
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Unexpected error hanging up call {CallId}. CorrelationId: {CorrelationId}", 
                    callId, correlationId);
                return StatusCode(500, new 
                { 
                    error = "An unexpected error occurred",
                    correlationId = correlationId
                });
            }
        }

        [HttpGet("{callId}/status")]
        public async Task<IActionResult> GetCallStatus(int callId)
        {
            try
            {
                if (callId <= 0)
                {
                    return BadRequest(new { error = "Invalid call ID" });
                }

                var call = await _sipManager.GetCallStatusAsync(callId);
                if (call == null)
                {
                    return NotFound(new { error = "Call not found" });
                }

                return Ok(new
                {
                    callId = call.CallId,
                    status = call.Status,
                    startedAt = call.StartedAt,
                    endedAt = call.EndedAt,
                    remoteUri = call.RemoteUri
                });
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Error getting call status for {CallId}. CorrelationId: {CorrelationId}", 
                    callId, correlationId);
                return StatusCode(500, new 
                { 
                    error = "Failed to retrieve call status",
                    correlationId = correlationId
                });
            }
        }
    }
}
