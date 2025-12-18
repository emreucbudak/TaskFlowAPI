using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public record DeleteSubTasksCommandRequest : IRequest
    {
        public Guid SubTaskId { get; init; }
        public Guid TaskId { get; init; }
    }
}
