using FlashMediator;
using Stats.Application.Features.CQRS.WorkerStats.Exceptions;
using Stats.Application.Repositories;

namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetByUserAndPeriod
{
    public class GetWorkerStatsByUserAndPeriodQueryHandler : IRequestHandler<GetWorkerStatsByUserAndPeriodQueryRequest, GetWorkerStatsByUserAndPeriodQueryResponse>
    {
        private readonly IWorkerStatsReadRepositories _readRepository;

        public GetWorkerStatsByUserAndPeriodQueryHandler(IWorkerStatsReadRepositories readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<GetWorkerStatsByUserAndPeriodQueryResponse> Handle(GetWorkerStatsByUserAndPeriodQueryRequest request, CancellationToken cancellationToken)
        {
            var s = await _readRepository.GetByUserAndPeriodAsync(request.UserId, request.Period, false);

            if (s is null)
            {
                throw new WorkerStatsNotFoundExceptions(request.Period);
            }

            return new GetWorkerStatsByUserAndPeriodQueryResponse(
                s.Id,
                s.UserId,
                s.Period,
                s.TotalTasksAssigned,
                s.TotalTasksCompleted,
                s.TasksCompletedBeforeDeadline,
                s.OverdueIncompleteTasksCount,
                s.TotalPoints);
        }
    }
}
