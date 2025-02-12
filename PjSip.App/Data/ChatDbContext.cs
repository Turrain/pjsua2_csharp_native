using ChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationships

            modelBuilder.Entity<Agent>()
                .HasMany(a => a.Chats)
                .WithOne()
                .HasForeignKey(c => c.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne()
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
