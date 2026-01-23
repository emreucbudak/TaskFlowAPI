using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Queries.GetAll
{
    public record GetAllTaskAnswerQueriesRequest : IRequest<List<GetAllTaskAnswerQueriesResponse>>
    {
        public Guid Id { get; init; }
    }
}
