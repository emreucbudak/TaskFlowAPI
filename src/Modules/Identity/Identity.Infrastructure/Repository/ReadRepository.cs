using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public class ReadRepository<T, TKey> : IReadRepository<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly IdentityManagementDbContext _context;

        public ReadRepository(IdentityManagementDbContext context)
        {
            _context = context;
        }
        private DbSet<T> db => _context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync(bool trackChanges, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            IQueryable<T> query = db.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();
            if (inc is not null)
            {
                query = inc(query);
            }

        
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(bool trackChanges, TKey id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            IQueryable<T> query = db.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();
            if (inc is not null)
            {
                query = inc(query);
            }


                return await query.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
    }
}
