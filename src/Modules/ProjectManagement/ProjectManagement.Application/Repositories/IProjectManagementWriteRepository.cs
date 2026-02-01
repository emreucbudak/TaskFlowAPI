using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository <T> where T : BaseEntity
    {
        Task AddTask (T entity);
        void DeleteTask (T task);
        void UpdateTask (T task);

    }
}
