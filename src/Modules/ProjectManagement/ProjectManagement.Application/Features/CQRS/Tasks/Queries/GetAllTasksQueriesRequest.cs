using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public record GetAllTasksQueriesRequest : IRequest<List<GetAllTasksQueriesResponse>>
    {
        public Guid CompanyId { get; init; }
    }
}
