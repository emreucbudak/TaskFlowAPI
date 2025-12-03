using TaskFlow.Domain.Bases;

namespace TaskFlow.Application.Repositories
{
    public interface IWriteRepository <T> : IBaseEntity
    {
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
