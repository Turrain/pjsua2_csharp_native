using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PjSip.App.Data;
using PjSip.App.Models;

namespace PjSip.App.Services
{
    public class AgentConfigService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AgentConfigService> _logger;

        public AgentConfigService(IServiceScopeFactory scopeFactory, ILogger<AgentConfigService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<AgentConfig>> GetAllAgentConfigsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
            return await context.AgentConfigs.AsNoTracking().ToListAsync();
        }

        public async Task<AgentConfig> GetAgentConfigByIdAsync(int id)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
            return await context.AgentConfigs.FindAsync(id);
        }

        public async Task<AgentConfig> CreateAgentConfigAsync(AgentConfig config)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
            context.AgentConfigs.Add(config);
            await context.SaveChangesAsync();
            return config;
        }

        public async Task UpdateAgentConfigAsync(AgentConfig config)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
            context.AgentConfigs.Update(config);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAgentConfigAsync(int id)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SipDbContext>();
            var config = await context.AgentConfigs.FindAsync(id);
            if (config != null)
            {
                context.AgentConfigs.Remove(config);
                await context.SaveChangesAsync();
            }
        }
    }
}