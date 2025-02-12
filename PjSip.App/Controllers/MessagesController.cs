using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChatApi.Data;
using ChatApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApi.Controllers
{
    [Route("api/agents/{agentId}/chats/{chatId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<MessagesController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public MessagesController(ChatDbContext context, ILogger<MessagesController> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/agents/{agentId}/chats/{chatId}/messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int agentId, int chatId)
        {
            // Ensure the chat exists under the specified agent
            if (!await _context.Chats.AnyAsync(c => c.Id == chatId && c.AgentId == agentId))
            {
                return NotFound("Chat not found.");
            }

            return await _context.Messages.Where(m => m.ChatId == chatId).ToListAsync();
        }

        // POST: api/agents/{agentId}/chats/{chatId}/messages
        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(int agentId, int chatId, Message message)
        {
            // Validate that the chat exists
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == chatId && c.AgentId == agentId);
            if (chat == null)
            {
                return NotFound("Chat not found.");
            }

            // Set the chat ID and timestamp
            message.ChatId = chatId;
            message.Timestamp = DateTime.UtcNow;

            // Save the incoming message (from user or agent)
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // OPTIONAL: If the message is from a user, call the Ollama LLM to generate a response.
            if (message.Sender.ToLower() == "user")
            {
                // Get the associated agent for LLM settings.
                var agent = await _context.Agents.FindAsync(agentId);
                if (agent != null)
                {
                    var llmResponse = await QueryOllamaAsync(agent, message.Content);
                    
                    // Save the LLM response as a new message
                    var responseMessage = new Message
                    {
                        ChatId = chatId,
                        Sender = "agent",
                        Content = llmResponse,
                        Timestamp = DateTime.UtcNow
                    };

                    _context.Messages.Add(responseMessage);
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction(nameof(GetMessages), new { agentId, chatId }, message);
        }

        // DELETE: api/agents/{agentId}/chats/{chatId}/messages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int agentId, int chatId, int id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id && m.ChatId == chatId);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Sends a query to the Ollama LLM agent.
        /// </summary>
        private async Task<string> QueryOllamaAsync(Agent agent, string prompt)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMilliseconds(agent.Timeout);

                // Prepare the request payload. Adjust according to Ollama's API requirements.
                var requestData = new { prompt, config = agent.AdditionalConfig };

                var response = await client.PostAsJsonAsync(agent.EndpointUrl, requestData);
                if (response.IsSuccessStatusCode)
                {
                    string? result = null;
                    return result ?? "No response from LLM.";
                }
                else
                {
                    _logger.LogError("Error calling Ollama API. Status: {StatusCode}", response.StatusCode);
                    return "Error communicating with LLM.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when calling Ollama API.");
                return "Exception occurred while communicating with LLM.";
            }
        }
    }
}
