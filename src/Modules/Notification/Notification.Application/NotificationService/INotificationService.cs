
using Notification.Domain.Models;

namespace Notification.Application
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync (string userId, NotificationMessage nm);
    }
}
