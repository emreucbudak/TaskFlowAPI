using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById
{
    public record GetIndividualTaskByIdQueryRequest : IRequest<GetIndividualTaskByIdQueryResponse> , ICacheableQuery
    {
        public Guid Id { get; init; }

        public string CacheKey => "getoneindividualtasks";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
