using FlashMediator;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId
{
    public record GetIndividualTasksByUserIdQueryRequest : IRequest<PagedResult<GetIndividualTasksByUserIdQueryResponse>> 
    {
        public Guid UserId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public string CacheKey => $"getuserindividualtasks_{UserId}_{PageNumber}_{PageSize}";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    };
}

