using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Delete
{
    public record DeleteSubTaskAnswerCommandRequest : IRequest
    {
        public Guid TaskId  { get; init; }
        public Guid SubTaskId { get; init; }
        public Guid SubTaskAnswerId { get; init; }

    }
}
