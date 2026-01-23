using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Repositories
{
    public class NotificationWriteRepository(NotificationDbContext _db) : INotificationWriteRepository
    {
        public async Task SendNotification(NotificationMessage notification)
        {
            await _db.notificationMessages.AddAsync(notification);
            await _db.SaveChangesAsync();
        }
    }
}
