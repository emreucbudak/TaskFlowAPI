using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll
{
    public record GetAllSubTasksQueriesRequest :IRequest<List<GetAllSubTasksQueriesResponse>>
    {
        public Guid TaskId { get; init; }
    }
}
