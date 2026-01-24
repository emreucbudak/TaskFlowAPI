using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public class BaseReadRepository<T> : IBaseReadRepository<T> where T : BaseEntity
    {
        private readonly IdentityManagementDbContext _context;

        public BaseReadRepository(IdentityManagementDbContext context)
        {
            _context = context;
        }
        private DbSet<T> db => _context.Set<T>();

        public async Task<T> GetByGuidAsync(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            IQueryable<T> query = db.AsQueryable();
            if (inc != null)
            {
                query = inc(query);
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
