using TaskFlow.BuildingBlocks.UnitOfWork;
using ProjectManagement.Persistence.Data.ProjectManagementDb;

namespace ProjectManagement.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ProjectManagementDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
