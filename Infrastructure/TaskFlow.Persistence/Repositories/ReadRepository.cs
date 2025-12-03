using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;
using TaskFlow.Application.Repositories;
using TaskFlow.Persistence.ApplicationContext;

namespace TaskFlow.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, new()
    {
        private readonly ApplicationDbContext context;

        public ReadRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public DbSet<T> dbSet { get =>  context.Set<T>(); }
        public async Task<IList<T>> GetAllAsync(bool trackChange, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> ordered = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includable = null)
        {
            IQueryable<T> query = dbSet;
            if(!trackChange) { query = query.AsNoTracking(); }
            if(expression is not null) { query = query.Where(expression); }
            if(includable is not null)
            {
                query = includable(query);
            }
            if (ordered is not null) { query = ordered(query); }
            return await query.ToListAsync();
           
        }
        public async Task<IList<T>> GetAllAsyncWithPaging(bool trackChange, int pageSize, int pageNumber, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> ordered = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includable = null)
        {
            IQueryable<T> query = dbSet;
            if (!trackChange) { query = query.AsNoTracking(); }
            if (expression is not null) { query = query.Where(expression); }
            if (includable is not null) { query = includable(query); }
            if (ordered is not null) query = ordered(query);
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetByExpression(Expression<Func<T, bool>> expression, bool trackChanges, Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            IQueryable<T> query = dbSet;
            if (!trackChanges) { query = query.AsNoTracking(); }
            if (expression is not null) { query= query.Where(expression); }
            if (include is not null) { query = include(query); }
            return await query.FirstOrDefaultAsync();
        }
    }
}
