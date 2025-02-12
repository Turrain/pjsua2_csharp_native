using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PjSip.App.Models;

namespace PjSip.App.Data
{
    public class SipDbContext : DbContext
    {
        public DbSet<SipAccount> SipAccounts { get; set; }
        public DbSet<SipCall> SipCalls { get; set; }
        public DbSet<AgentConfig> AgentConfigs { get; set; }

        public SipDbContext(DbContextOptions<SipDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SipAccount>()
                .HasMany(a => a.Calls)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.SipAccountId);

            modelBuilder.Entity<AgentConfig>()
                .Property(a => a.LLMConfig)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()));

            modelBuilder.Entity<AgentConfig>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
