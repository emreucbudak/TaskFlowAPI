using TaskFlow.BuildingBlocks.UnitOfWork;
using Chat.Persistence.Data.ChatDb;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ChatDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}