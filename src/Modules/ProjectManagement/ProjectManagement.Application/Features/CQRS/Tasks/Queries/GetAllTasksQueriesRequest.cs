using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public record GetAllTasksQueriesRequest : IRequest<List<GetAllTasksQueriesRequest>>
    {
        public Guid CompanyId { get; init; }
    }
}
