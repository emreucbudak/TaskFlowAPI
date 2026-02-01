using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementReadRepository <T> where T : BaseEntity
    {
        Task<T> GetTask(Guid id,bool trackChanges);
        Task<List<T>> GetAllTasks(bool trackChanges,int pageSize);
    }
}
