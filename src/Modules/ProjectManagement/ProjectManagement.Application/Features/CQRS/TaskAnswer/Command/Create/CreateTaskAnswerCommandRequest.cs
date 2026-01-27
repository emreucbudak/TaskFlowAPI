using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Create
{
    public record CreateTaskAnswerCommandRequest : IRequest
    {
        public string AnswerText { get; init; }
        public Guid SenderId { get; init; }
        public Guid TaskId { get; init; }
    }
}
