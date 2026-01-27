using Identity.Infrastructure.Data.IdentityDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityManagementDbContext _context;

        public UnitOfWork(IdentityManagementDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
