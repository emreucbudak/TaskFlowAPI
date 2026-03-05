using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Application.Repositories
{
    public interface IReadRepository<T, TKey> where T : BaseEntity<TKey>
    {
        Task<T> GetByIdAsync(bool trackChanges, TKey id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null);
        Task<PagedResult<T>> GetAllAsync(
            int pageSize,
            int page = 1,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null,
            Expression<Func<T, bool>>? predicate = null);
    }
}
