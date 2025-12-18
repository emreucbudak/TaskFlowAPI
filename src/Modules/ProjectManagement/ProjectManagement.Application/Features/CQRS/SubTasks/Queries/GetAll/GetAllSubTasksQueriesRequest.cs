using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll
{
    public class GetAllSubTasksQueriesRequest :IRequest<List<GetAllSubTasksQueriesResponse>>
    {
        public Guid TaskId { get; set; }
    }
}
