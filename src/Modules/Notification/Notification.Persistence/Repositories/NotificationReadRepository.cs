using Microsoft.EntityFrameworkCore;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Repositories
{
    public class NotificationReadRepository : INotificationReadRepository
    {
        private readonly NotificationDbContext context;

        public NotificationReadRepository(NotificationDbContext context)
        {
            this.context = context;
        }

        public async Task<NotificationMessage> GetByIdAsync(bool trackChanges, Guid userId, Guid notificationId)
        {
            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.Id == notificationId && x.ReceiverUserId == userId); 

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<NotificationMessage>> GetByUserIdAsync(Guid userId, bool trackChanges = false)
        {
            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId)
                .OrderByDescending(x => x.SendTime);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x => x.ReceiverUserId == userId && !x.IsRead);
        }
    }
}
