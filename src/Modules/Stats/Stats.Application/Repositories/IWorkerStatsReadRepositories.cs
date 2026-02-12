using Stats.Domain.Entities;
using TaskFlow.BuildingBlocks.Common;

namespace Stats.Application.Repositories
{
    public interface IWorkerStatsReadRepositories
    {
        Task<WorkerStats?> GetByUserAndPeriodAsync(Guid userId, DateOnly period, bool trackChanges);
        Task<PagedResult<WorkerStats>> GetAllWorkersStatsByPeriodAsync(DateOnly period, int page, int pageSize, bool trackChanges);
    }
}