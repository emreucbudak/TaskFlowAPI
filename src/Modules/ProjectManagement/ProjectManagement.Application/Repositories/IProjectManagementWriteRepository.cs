namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository
    {
        Task AddTask (ProjectManagement.Domain.Entities.Task task);
        void DeleteTask (ProjectManagement.Domain.Entities.Task task);
        void UpdateTask (ProjectManagement.Domain.Entities.Task task);

    }
}
