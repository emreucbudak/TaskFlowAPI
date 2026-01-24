using TaskFlow.BuildingBlocks.UnitOfWork;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.UnitOfWork
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
