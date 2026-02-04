using TaskFlow.BuildingBlocks.UnitOfWork;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(NotificationDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
