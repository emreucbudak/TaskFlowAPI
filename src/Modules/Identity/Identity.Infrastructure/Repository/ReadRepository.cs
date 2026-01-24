using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Identity.Infrastructure.Repository
{
    public class ReadRepository<T> : IReadRepository<T> where T : class
    {
        private readonly IdentityManagementDbContext _context;

        public ReadRepository(IdentityManagementDbContext context)
        {
            _context = context;
        }
        private DbSet<T> db => _context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var getAll = await db.ToListAsync();
            return getAll;
        }





        public async Task<T> GetByIdAsync(int id)
        {
            var getById = await db.FindAsync(id);
            return getById;
        }


    }
}
