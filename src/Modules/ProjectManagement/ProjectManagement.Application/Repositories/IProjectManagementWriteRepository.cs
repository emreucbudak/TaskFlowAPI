using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository <T> where T : BaseEntity
    {
        Task AddTask (T entity);
        Task DeleteTask (T task);
        Task UpdateTask (T task);

    }
}
