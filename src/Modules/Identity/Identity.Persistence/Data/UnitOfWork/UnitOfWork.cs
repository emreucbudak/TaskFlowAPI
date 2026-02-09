using Identity.Persistence.Data.IdentityDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(IdentityManagementDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
