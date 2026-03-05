using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Application.Repositories
{
    public interface IProjectManagementReadRepository
    {
        System.Threading.Tasks.Task<Task> GetTask(Guid id, bool trackChanges, CancellationToken cancellationToken);
        System.Threading.Tasks.Task<(List<Task> Items, int TotalCount)> GetAllTasks(bool trackChanges, int pageNumber, int pageSize, CancellationToken cancellationToken);
        System.Threading.Tasks.Task<Domain.Entities.IndividualTasks> GetIndividualTask(Guid id, bool trackChanges, CancellationToken cancellationToken);
        System.Threading.Tasks.Task<(List<Domain.Entities.IndividualTasks> Items, int TotalCount)> GetAllIndividualTasks(bool trackChanges, int pageNumber, int pageSize, Guid userId, CancellationToken cancellationToken);
    }
}
