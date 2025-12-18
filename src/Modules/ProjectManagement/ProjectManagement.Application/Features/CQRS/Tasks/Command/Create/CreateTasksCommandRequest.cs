using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Create
{
    public record CreateTasksCommandRequest : IRequest
    {
        public string TaskName { get; init; }
        public string Description { get; init; }
        public DateTime DeadlineTime { get; init; }
    }
}
