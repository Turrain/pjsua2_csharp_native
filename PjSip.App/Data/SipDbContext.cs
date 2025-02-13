using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using PjSip.App.Models;

namespace PjSip.App.Data
{
    public class SipDbContextFactory : IDesignTimeDbContextFactory<SipDbContext>
{
    public SipDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SipDbContext>();
        optionsBuilder.UseSqlite("Data Source=chat.db");
        return new SipDbContext(optionsBuilder.Options);
    }
}
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
        .HasIndex(a => a.AccountId)
        .IsUnique();
            modelBuilder.Entity<SipAccount>()
                .HasMany(a => a.Calls)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.SipAccountId);

            modelBuilder.Entity<AgentConfig>()
                .Property(a => a.LLMConfig)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()))
                .Metadata.SetValueComparer(
                    new ValueComparer<Dictionary<string, string>>(
                        (d1, d2) => d1.SequenceEqual(d2),
                        d => d.Aggregate(0, (hash, pair) => HashCode.Combine(hash, pair.GetHashCode())),
                        d => new Dictionary<string, string>(d)
                    ));

            modelBuilder.Entity<AgentConfig>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
