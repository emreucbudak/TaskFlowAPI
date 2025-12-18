using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask
{
    public record UpdateTaskCommandRequest : IRequest
    {
        public string TaskName { get; init; }
        public string Description { get; init; } 
        public DateTime DeadlineTime { get; init; }
        public Guid TaskId { get; init; }
    }
}
