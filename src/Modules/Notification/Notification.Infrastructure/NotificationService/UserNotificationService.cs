using Notification.Application;
using Notification.Domain.Models;

namespace Notification.Infrastructure.NotificationService
{
    public class UserNotificationService : INotificationService
    {
        public Task SendNotificationToUserAsync(string userId, NotificationMessage nm)
        {
            throw new NotImplementedException();
        }
    }
}
