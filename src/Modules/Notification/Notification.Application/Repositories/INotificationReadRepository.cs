using Notification.Domain.Models;
using TaskFlow.BuildingBlocks.Common;

namespace Notification.Application.Repositories
{
    public interface INotificationReadRepository
    {
        Task<NotificationMessage?> GetByIdAsync(bool trackChanges, Guid userId, Guid notificationId);
        Task<PagedResult<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int pageSize,
            int page = 1,
            bool trackChanges = false);
        Task<IReadOnlyList<NotificationMessage>> GetUnreadByUserIdAsync(
            Guid userId,
            int maxCount,
            bool trackChanges = false);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
