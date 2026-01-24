using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Application.Repositories
{
    public interface IReadRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

    }
}
