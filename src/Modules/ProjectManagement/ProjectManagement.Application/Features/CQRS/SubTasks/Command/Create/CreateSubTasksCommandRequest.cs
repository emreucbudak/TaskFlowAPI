using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create
{
    public record CreateSubTasksCommandRequest : IRequest
    {
        public Guid TaskId { get; init; }
        public string Description { get; init; }
        public Guid AssignedUserId  { get; init; }
        public string TaskTitle { get; init; }
    }
}
