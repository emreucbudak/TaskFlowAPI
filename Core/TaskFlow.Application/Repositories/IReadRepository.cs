using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TaskFlow.Domain.Bases;

namespace TaskFlow.Application.Repositories
{
    public interface IReadRepository<T> : IBaseEntity
    {
        Task<IList<T>> GetAllAsync(bool trackChange, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> ordered = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includable= null);
        Task<IList<T>> GetAllAsyncWithPaging(bool trackChange, int pageSize, int pageNumber, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> ordered = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includable = null);
        Task<T> GetByExpression (Expression<Func<T,bool>> expression,bool trackChanges,Func<IQueryable<T>,IIncludableQueryable<T,object>>include);
    }
}
