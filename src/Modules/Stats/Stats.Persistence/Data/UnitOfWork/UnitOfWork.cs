using TaskFlow.BuildingBlocks.UnitOfWork;
using Stats.Persistence.Data;

namespace Stats.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(StatsDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
