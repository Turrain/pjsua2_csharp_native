using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using PjSip.App.Models;
using PjSip.App.Services;

namespace PjSip.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentConfigController : ControllerBase
    {
        private readonly AgentConfigService _agentConfigService;

        public AgentConfigController(AgentConfigService agentConfigService)
        {
            _agentConfigService = agentConfigService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentConfig>>> GetConfigs()
        {
            var configs = await _agentConfigService.GetAllAgentConfigsAsync();
            return Ok(configs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AgentConfig>> GetConfig(int id)
        {
            var config = await _agentConfigService.GetAgentConfigByIdAsync(id);
            if (config == null)
            {
                return NotFound();
            }
            return Ok(config);
        }

        [HttpPost]
        public async Task<ActionResult<AgentConfig>> CreateConfig([FromBody] AgentConfig config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdConfig = await _agentConfigService.CreateAgentConfigAsync(config);
            return CreatedAtAction(nameof(GetConfig), new { id = createdConfig.Id }, createdConfig);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] AgentConfig config)
        {
            if (id != config.Id)
            {
                return BadRequest("Mismatched config id");
            }

            await _agentConfigService.UpdateAgentConfigAsync(config);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            await _agentConfigService.DeleteAgentConfigAsync(id);
            return NoContent();
        }
    }
}