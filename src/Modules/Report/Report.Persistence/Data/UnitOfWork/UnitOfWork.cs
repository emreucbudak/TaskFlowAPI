using TaskFlow.BuildingBlocks.UnitOfWork;
using Report.Persistence.Data;

namespace Report.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ReportDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
