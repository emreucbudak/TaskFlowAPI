using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Application.Repositories
{
    public interface IReadRepository<T, TKey> where T : BaseEntity<TKey>
    {
        Task<T> GetByIdAsync(bool trackChanges,TKey id,Func<IQueryable<T>,IIncludableQueryable<T,object>>? inc = null);
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null);

    }
}
