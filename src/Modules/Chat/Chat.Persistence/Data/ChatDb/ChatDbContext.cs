using Chat.Domain.Entities;
using Chat.Persistence.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Chat.Persistence.Data.ChatDb
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Chat");
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}