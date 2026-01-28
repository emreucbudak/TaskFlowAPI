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

        public async Task<List<NotificationMessage>> GetByUserIdAsync(Guid userId, int take, bool trackChanges = false)
        {
            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId)
                .OrderByDescending(x => x.SendTime);

            if (!trackChanges)
                query = query.AsNoTracking();

            if(take > 200)
                take = 200;
            if (take <1)
                take = 1;
            query = query.Take(take);


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
