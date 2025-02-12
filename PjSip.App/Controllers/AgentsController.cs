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
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(ChatDbContext context, ILogger<AgentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/agents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            return await _context.Agents.ToListAsync();
        }

        // GET: api/agents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Agent>> GetAgent(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return agent;
        }

        // POST: api/agents
        [HttpPost]
        public async Task<ActionResult<Agent>> CreateAgent(Agent agent)
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, agent);
        }

        // PUT: api/agents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgent(int id, Agent agent)
        {
            if (id != agent.Id)
            {
                return BadRequest("Agent ID mismatch.");
            }

            _context.Entry(agent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Agents.Any(a => a.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency error updating agent with ID {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/agents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgent(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
