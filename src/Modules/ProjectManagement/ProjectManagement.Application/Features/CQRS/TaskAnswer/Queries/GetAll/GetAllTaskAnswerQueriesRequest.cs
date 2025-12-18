using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Queries.GetAll
{
    public record GetAllTaskAnswerQueriesRequest : IRequest<List<GetAllTaskAnswerQueriesResponse>>
    {
        public Guid Id { get; init; }
    }
}
