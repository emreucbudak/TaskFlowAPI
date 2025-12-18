using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll
{
    public record GetAllSubTaskAnswerQueriesRequest : IRequest<List<GetAllSubTaskAnswerQueriesResponse>>
    {
        public Guid TaskId { get; init; }
        public Guid SubTaskId   { get; init; }
    }
}
