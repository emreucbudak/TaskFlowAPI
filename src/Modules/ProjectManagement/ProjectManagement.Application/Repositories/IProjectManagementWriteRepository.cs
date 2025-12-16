namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository
    {
        Task AddTask (ProjectManagement.Domain.Entities.Task task);
        Task DeleteTask (ProjectManagement.Domain.Entities.Task task);
        Task<ProjectManagement.Domain.Entities.Task>  GetTask (Guid id);
        Task UpdateTask (ProjectManagement.Domain.Entities.Task task);

    }
}
