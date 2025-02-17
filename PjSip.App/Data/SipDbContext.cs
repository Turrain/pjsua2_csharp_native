using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PjSip.App.Models;
using System.Text.Json;

namespace PjSip.App.Data;

public class SipDbContext : DbContext
{
    public SipDbContext(DbContextOptions<SipDbContext> options) : base(options) { }

    public DbSet<SipAccount> SipAccounts => Set<SipAccount>();
    public DbSet<SipCall> SipCalls => Set<SipCall>();
    public DbSet<AgentConfig> AgentConfigs => Set<AgentConfig>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SipAccount Configuration
        modelBuilder.Entity<SipAccount>(entity =>
        {
            entity.HasIndex(a => a.AccountId).IsUnique();
            entity.Property(a => a.AccountId).HasMaxLength(255).IsRequired();
            entity.HasMany(a => a.Calls)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.SipAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SipCall Configuration
        modelBuilder.Entity<SipCall>(entity =>
        {
            entity.Property(c => c.RemoteUri).HasMaxLength(511).IsRequired();
            entity.Property(c => c.Status).HasMaxLength(50).IsRequired();
        });

        // AgentConfig Configuration
      modelBuilder.Entity<AgentConfig>(entity =>
{
    entity.Property(a => a.LLMConfig)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)!);

    entity.Property(a => a.CreatedAt).HasDefaultValueSql("DATETIME('now')");
    entity.Property(a => a.AgentId).HasMaxLength(255).IsRequired();
    entity.Property(a => a.Model).HasMaxLength(255).IsRequired();
});
        // Message Configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(m => m.Sender).HasMaxLength(255).IsRequired();
            entity.Property(m => m.Content).IsRequired();
        });
    }
}

public class SipDbContextFactory : IDesignTimeDbContextFactory<SipDbContext>
{
     public SipDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SipDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

        return new SipDbContext(optionsBuilder.Options);
    }
}