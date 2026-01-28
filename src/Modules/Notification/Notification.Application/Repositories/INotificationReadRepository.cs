using Notification.Domain.Models;

namespace Notification.Application.Repositories
{
    public interface INotificationReadRepository
    {
        Task<Notification.Domain.Models.NotificationMessage> GetByIdAsync(bool trackChanges,Guid userId,Guid notificationId);
        Task<List<NotificationMessage>> GetByUserIdAsync(Guid userId, bool trackChanges = false);
        Task<int> GetUnreadCountAsync(Guid userId);


    }
}
