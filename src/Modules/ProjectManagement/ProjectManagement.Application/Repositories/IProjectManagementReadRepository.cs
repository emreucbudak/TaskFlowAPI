using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementReadRepository
    {
        System.Threading.Tasks.Task<Task> GetTask(Guid id,bool trackChanges);
        System.Threading.Tasks.Task<List<Task>> GetAllTasks(bool trackChanges,int pageNumber,int pageSize);
        System.Threading.Tasks.Task<Domain.Entities.IndividualTasks> GetIndividualTask(Guid id, bool trackChanges);
        System.Threading.Tasks.Task<List<Domain.Entities.IndividualTasks>> GetAllIndividualTasks(bool trackChanges, int pageNumber, int pageSize);
        System.Threading.Tasks.Task<List<Domain.Entities.IndividualTasks>> GetIndividualTasksByUserId(Guid userId, bool trackChanges);
    }
}
