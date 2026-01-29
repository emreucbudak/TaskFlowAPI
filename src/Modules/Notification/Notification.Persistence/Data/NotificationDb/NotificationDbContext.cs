using Microsoft.EntityFrameworkCore;
using Notification.Domain.Models;

namespace Notification.Infrastructure.Data.NotificationDb
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected NotificationDbContext()
        {
        }
        public DbSet<NotificationMessage> notificationMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var notification = modelBuilder.Entity<NotificationMessage>();

            notification.HasIndex(n => new { n.ReceiverUserId, n.SendTime })
                .IsDescending(false, true);


        }
    }
}
