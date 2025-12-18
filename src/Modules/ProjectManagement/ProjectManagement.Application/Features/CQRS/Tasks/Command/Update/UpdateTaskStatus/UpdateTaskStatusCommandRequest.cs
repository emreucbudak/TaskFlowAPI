using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTaskStatus
{
    public record UpdateTaskStatusCommandRequest : IRequest
    {
        public int TaskStatusId { get; init; }
        public Guid TaskId { get; init; }
    }
}
