using Microsoft.EntityFrameworkCore;
using Notification.Domain.Models;
using Notification.Persistence.Data.DataConfiguration;

namespace Notification.Infrastructure.Data.NotificationDb
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        protected NotificationDbContext()
        {
        }
        public DbSet<NotificationMessage> NotificationMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Notification");
            modelBuilder.ApplyConfiguration(new NotificationMessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}