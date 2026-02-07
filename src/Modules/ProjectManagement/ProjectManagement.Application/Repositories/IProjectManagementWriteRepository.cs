using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementWriteRepository
    {
        System.Threading.Tasks.Task AddTask (Task entity);
        System.Threading.Tasks.Task DeleteTask (Task task);
        System.Threading.Tasks.Task UpdateTask (Task task);
        System.Threading.Tasks.Task AddIndividualTask(Domain.Entities.IndividualTasks entity);
        System.Threading.Tasks.Task DeleteIndividualTask(Domain.Entities.IndividualTasks task);
        System.Threading.Tasks.Task UpdateIndividualTask(Domain.Entities.IndividualTasks task);
    }
}
