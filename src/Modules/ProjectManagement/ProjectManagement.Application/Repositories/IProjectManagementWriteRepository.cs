using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository
    {
        System.Threading.Tasks.Task AddTask (Task entity);
        System.Threading.Tasks.Task DeleteTask (Task task);
        System.Threading.Tasks.Task UpdateTask (Task task);

    }
}
