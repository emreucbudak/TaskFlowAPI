using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Delete
{
    public record DeleteTaskAnswerCommandRequest : IRequest
    {
        public Guid TaskAnswerId { get; init; }
        public Guid TaskId { get; init; }
    }
}
