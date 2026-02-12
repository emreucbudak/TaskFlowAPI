using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetByUserAndPeriod
{
    public record GetWorkerStatsByUserAndPeriodQueryRequest : IRequest<GetWorkerStatsByUserAndPeriodQueryResponse> , ICacheableQuery
    {
        public Guid UserId { get; init; }
        public DateOnly Period { get; init; }

        public string CacheKey => "getworkerstatsbyuseridandperiod";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
