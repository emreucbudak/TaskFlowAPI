using Chat.Persistence.Data.ChatDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ChatDbContext context) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}