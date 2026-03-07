using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById
{
    public record GetIndividualTaskByIdQueryRequest : IRequest<GetIndividualTaskByIdQueryResponse> 
    {
        public Guid Id { get; init; }

        public string CacheKey => "getoneindividualtasks";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
    }
}

