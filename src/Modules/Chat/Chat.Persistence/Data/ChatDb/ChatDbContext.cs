using Microsoft.EntityFrameworkCore;
using Chat.Domain.Entities;
using Chat.Persistence.Data.DataConfiguration;

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
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}