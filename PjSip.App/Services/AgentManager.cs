using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PjSip.App.Exceptions;
using PjSip.App.Utils;
using PjSip.App.Data;
using PjSip.App.Models;
using PjSip.App.Sip;

namespace PjSip.App.Services
{
    public class AgentManager : IDisposable
    {
        private readonly SipDbContext _context;
        private readonly ILogger<AgentManager> _logger;
        private readonly ConcurrentDictionary<int, MediaPort> _mediaPorts = new();
        private readonly HttpClient _httpClient;
        private readonly RetryPolicy _retryPolicy;
        private readonly CircuitBreaker _circuitBreaker;

        private const int MAX_RETRIES = 3;
        private const int RETRY_DELAY_MS = 1000;

        public AgentManager(SipDbContext context, ILogger<AgentManager> logger)
        {
            _context = context;
            _logger = logger;
            _httpClient = new HttpClient();
            _retryPolicy = new RetryPolicy(MAX_RETRIES, RETRY_DELAY_MS);
            _circuitBreaker = new CircuitBreaker(
                failureThreshold: 5,
                resetTimeout: TimeSpan.FromMinutes(1)
            );
        }

        public void RegisterMediaPort(int callId, MediaPort port)
        {
            try
            {
                if (_mediaPorts.ContainsKey(callId))
                {
                    _logger.LogWarning("MediaPort already registered for call {CallId}", callId);
                    return;
                }

                _mediaPorts[callId] = port;
                port.VoiceActivityDetected += async (data) => await ProcessAudioSafe(callId, data);
                _logger.LogInformation("MediaPort registered for call {CallId}", callId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register MediaPort for call {CallId}", callId);
                throw;
            }
        }

        private async Task ProcessAudioSafe(int callId, byte[] audioData)
        {
            if (!_circuitBreaker.CanExecute())
            {
                _logger.LogWarning("Circuit breaker open, skipping audio processing for call {CallId}", callId);
                return;
            }

            try
            {
                var call = await _context.SipCalls
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.CallId == callId);

                if (call == null)
                {
                    _logger.LogWarning("Call {CallId} not found", callId);
                    return;
                }

                var agentConfig = await _context.AgentConfigs
                    .FirstOrDefaultAsync(a => a.Id == call.Account.Id);

                if (agentConfig == null)
                {
                    _logger.LogWarning("No agent config found for call {CallId}", callId);
                    return;
                }

                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await ProcessWithAI(agentConfig, audioData);
                    if (_mediaPorts.TryGetValue(callId, out var port))
                    {
                        port.Transmit(response);
                        _circuitBreaker.OnSuccess();
                    }
                });
            }
            catch (Exception ex)
            {
                _circuitBreaker.OnFailure();
                _logger.LogError(ex, "Audio processing failed for call {CallId}", callId);
                await UpdateCallStatus(callId, "ERROR");
            }
        }

        private async Task<byte[]> ProcessWithAI(AgentConfig config, byte[] audio)
        {
            try
            {
                // First, convert audio to text using Whisper
                using var whisperContent = new ByteArrayContent(audio);
                whisperContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
                var whisperResponse = await _httpClient.PostAsync(config.WhisperEndpoint, whisperContent);
                whisperResponse.EnsureSuccessStatusCode();
                var transcription = await whisperResponse.Content.ReadAsStringAsync();

                // Then, process with Ollama
                using var ollamaContent = new StringContent(transcription);
                ollamaContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                var ollamaResponse = await _httpClient.PostAsync(config.OllamaEndpoint, ollamaContent);
                ollamaResponse.EnsureSuccessStatusCode();
                var aiResponse = await ollamaResponse.Content.ReadAsStringAsync();

                // Finally, convert response to audio using Auralis
                using var auralisContent = new StringContent(aiResponse);
                auralisContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                var auralisResponse = await _httpClient.PostAsync(config.AuralisEndpoint, auralisContent);
                auralisResponse.EnsureSuccessStatusCode();

                return await auralisResponse.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AI service request failed: {Message}", ex.Message);
                throw new AIProcessingException(
                    "Failed to process audio with AI services",
                    ex.StatusCode?.ToString() ?? "UNKNOWN",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI processing failed");
                throw new AIProcessingException(
                    "Unexpected error during AI processing",
                    "INTERNAL_ERROR",
                    ex);
            }
        }

        public async Task UpdateCallStatus(int callId, string state)
        {
            try
            {
                var call = await _context.SipCalls.FindAsync(callId);
                if (call != null)
                {
                    call.Status = state;
                    if (state == "ERROR" || state == "TERMINATED")
                    {
                        call.EndedAt = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Call {CallId} status updated to {State}", callId, state);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update call status for {CallId}", callId);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _httpClient.Dispose();
                foreach (var port in _mediaPorts.Values)
                {
                    try
                    {
                        port.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing MediaPort");
                    }
                }
                _mediaPorts.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AgentManager disposal");
            }
        }
    }
}
