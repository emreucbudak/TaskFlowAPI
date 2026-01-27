using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Create
{
    public record CreateSubTaskAnswerCommandRequest : IRequest
    {
        public string AnswerText { get; init; }
        public Guid SenderId { get; init; }
        public Guid SubTaskId { get; init; }
        public Guid TaskId { get; init; }
    }
}
