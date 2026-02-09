using TaskFlow.BuildingBlocks.UnitOfWork;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TenantDbContext _context;

        public UnitOfWork(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
           return await _context.SaveChangesAsync();
        }
    }
}
