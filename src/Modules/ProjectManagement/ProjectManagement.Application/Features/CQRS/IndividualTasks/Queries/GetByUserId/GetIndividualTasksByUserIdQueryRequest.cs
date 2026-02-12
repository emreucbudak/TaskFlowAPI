using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId
{
    public record GetIndividualTasksByUserIdQueryRequest : IRequest<List<GetIndividualTasksByUserIdQueryResponse>> , ICacheableQuery
    {
        public Guid UserId { get; init; }

        public string CacheKey => "getuserindividualtasks";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    };
}
