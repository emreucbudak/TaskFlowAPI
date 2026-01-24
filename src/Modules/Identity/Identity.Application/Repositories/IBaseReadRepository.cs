using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Application.Repositories
{
    public interface IBaseReadRepository<T> where T : BaseEntity
    {
        Task<T> GetByGuidAsync(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null) ;
    }
}
