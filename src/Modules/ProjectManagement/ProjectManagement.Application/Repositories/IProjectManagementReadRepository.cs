namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementReadRepository
    {
        Task<ProjectManagement.Domain.Entities.Task> GetTask(Guid id,bool trackChanges);
        Task<List<ProjectManagement.Domain.Entities.Task>> GetAllTasks(bool trackChanges);
    }
}
