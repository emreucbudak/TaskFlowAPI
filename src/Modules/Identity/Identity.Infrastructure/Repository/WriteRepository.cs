using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Infrastructure.Repository
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class
    {
        private readonly IdentityManagementDbContext _context;
   

        public WriteRepository(IdentityManagementDbContext context)
        {
            _context = context;

        }
        private DbSet<T> db => _context.Set<T>();
        public async Task AddAsync(T entity)
        {
          await  db.AddAsync(entity);

        }

        public async Task DeleteAsync(T entity)
        {
            db.Remove(entity);

        }

        public async Task UpdateAsync(T entity)
        {
            db.Update(entity);

        }
    }
}
