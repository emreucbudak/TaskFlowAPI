using Identity.Infrastructure.Data.IdentityDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork(IdentityManagementDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}