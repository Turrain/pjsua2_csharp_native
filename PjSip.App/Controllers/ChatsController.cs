using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Data;
using ChatApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApi.Controllers
{
    [Route("api/agents/{agentId}/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(ChatDbContext context, ILogger<ChatsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/agents/{agentId}/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats(int agentId)
        {
            // Verify agent exists
            if (!await _context.Agents.AnyAsync(a => a.Id == agentId))
            {
                return NotFound("Agent not found.");
            }

            return await _context.Chats.Where(c => c.AgentId == agentId).ToListAsync();
        }

        // GET: api/agents/{agentId}/chats/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(int agentId, int id)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id && c.AgentId == agentId);
            if (chat == null)
            {
                return NotFound();
            }
            return chat;
        }

        // POST: api/agents/{agentId}/chats
        [HttpPost]
        public async Task<ActionResult<Chat>> CreateChat(int agentId, Chat chat)
        {
            // Verify agent exists
            if (!await _context.Agents.AnyAsync(a => a.Id == agentId))
            {
                return NotFound("Agent not found.");
            }

            chat.AgentId = agentId;
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChat), new { agentId = agentId, id = chat.Id }, chat);
        }

        // PUT: api/agents/{agentId}/chats/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChat(int agentId, int id, Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest("Chat ID mismatch.");
            }

            // Ensure the chat belongs to the agent
            if (chat.AgentId != agentId)
            {
                return BadRequest("Agent ID mismatch in chat data.");
            }

            _context.Entry(chat).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Chats.Any(c => c.Id == id && c.AgentId == agentId))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency error updating chat with ID {Id} for agent {AgentId}", id, agentId);
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/agents/{agentId}/chats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int agentId, int id)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id && c.AgentId == agentId);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
