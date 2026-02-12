using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetAll
{
    public record GetAllWorkersStatsByPeriodQueryRequest : IRequest<PagedResult<GetAllWorkersStatsByPeriodQueryResponse>> , ICacheableQuery
    {
        public DateOnly Period { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public string CacheKey => "getallworkerstats";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
