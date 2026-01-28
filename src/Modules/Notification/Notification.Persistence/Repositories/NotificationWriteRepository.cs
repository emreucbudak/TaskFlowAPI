using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Persistence.Repositories
{
    public class NotificationWriteRepository(NotificationDbContext _db,IUnitOfWork unit) : INotificationWriteRepository
    {
        public void DeleteNotification(NotificationMessage notification)
        {
             _db.notificationMessages.Remove(notification);
            

        }

        public async Task SendNotification(NotificationMessage notification)
        {
            await _db.notificationMessages.AddAsync(notification);
   
        }
    }
}
