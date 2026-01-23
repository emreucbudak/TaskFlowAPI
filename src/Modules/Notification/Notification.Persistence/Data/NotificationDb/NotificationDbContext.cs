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
    }
}
