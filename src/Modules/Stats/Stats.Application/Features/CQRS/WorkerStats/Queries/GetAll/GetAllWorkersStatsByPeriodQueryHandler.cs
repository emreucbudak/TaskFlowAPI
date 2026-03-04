using FlashMediator;
using Stats.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetAll
{
    public class GetAllWorkersStatsByPeriodQueryHandler : IRequestHandler<GetAllWorkersStatsByPeriodQueryRequest, PagedResult<GetAllWorkersStatsByPeriodQueryResponse>>
    {
        private readonly IWorkerStatsReadRepositories _readRepository;

        public GetAllWorkersStatsByPeriodQueryHandler(IWorkerStatsReadRepositories readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PagedResult<GetAllWorkersStatsByPeriodQueryResponse>> Handle(GetAllWorkersStatsByPeriodQueryRequest request, CancellationToken cancellationToken)
        {
            var stats = await _readRepository.GetAllWorkersStatsByPeriodAsync(request.Period, request.Page, request.PageSize, false);

            var items = stats.Items.Select(s => new GetAllWorkersStatsByPeriodQueryResponse(
                s.Id,
                s.UserId,
                s.Period,
                s.TotalTasksAssigned,
                s.TotalTasksCompleted,
                s.TasksCompletedBeforeDeadline,
                s.OverdueIncompleteTasksCount,
                s.TotalPoints)).ToList();

            return new PagedResult<GetAllWorkersStatsByPeriodQueryResponse>
            {
                Items = items,
                TotalCount = stats.TotalCount,
                Page = stats.Page,
                PageSize = stats.PageSize
            };
        }
    }
}
