using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public sealed record GetAllTasksQueriesRequest : IRequest<PagedResult<GetAllTasksQueriesResponse>>, ICacheableQuery
    {
        public Guid CompanyId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public string CacheKey => $"getalltasks:{CompanyId}:{PageNumber}:{PageSize}";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
