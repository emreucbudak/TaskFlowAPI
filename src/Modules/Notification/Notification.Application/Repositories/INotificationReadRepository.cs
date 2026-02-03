using Notification.Domain.Models;
using TaskFlow.BuildingBlocks.Common;

namespace Notification.Application.Repositories
{
    public interface INotificationReadRepository
    {
        Task<Notification.Domain.Models.NotificationMessage> GetByIdAsync(bool trackChanges,Guid userId,Guid notificationId);
        Task<PagedResult<NotificationMessage>> GetByUserIdAsync(Guid userId,
            int pageSize,
            int page = 1,
            bool trackChanges = false);
        Task<int> GetUnreadCountAsync(Guid userId);


    }
}
