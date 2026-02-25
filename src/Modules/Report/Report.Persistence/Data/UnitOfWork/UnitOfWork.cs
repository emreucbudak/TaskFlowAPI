using TaskFlow.BuildingBlocks.UnitOfWork;
using Report.Persistence.Data;
using Report.Application.UnitOfWork;

namespace Report.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ReportDbContext context) : IReportUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
