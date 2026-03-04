using Stats.Domain.Entities;

namespace Stats.Application.Repositories
{
    public interface IWorkerStatsWriteRepositories
    {
        void Update(WorkerStats workerStats);
        void Delete(WorkerStats workerStats);
        Task<WorkerStats> GetOrCreateStatsAsync(Guid userId, DateOnly period);
        Task RecordTaskCompletionAsync(Guid userId, DateOnly completedOn, DateOnly deadline, CancellationToken cancellationToken = default);
    }
}
