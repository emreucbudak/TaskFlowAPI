using Stats.Domain.Entities;

namespace Stats.Application.Repoitories
{
    public interface IWorkerStatsWriteRepositories
    {
        void Update(WorkerStats workerStats);
        void Delete(WorkerStats workerStats);
        Task<WorkerStats> GetOrCreateStatsAsync(Guid userId, DateOnly period);
    }
}
