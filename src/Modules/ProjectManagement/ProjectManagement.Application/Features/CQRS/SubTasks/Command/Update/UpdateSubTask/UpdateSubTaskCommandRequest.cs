using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask
{
    public record UpdateSubTaskCommandRequest : IRequest
    {
        public string Description { get; init; }
        public string TaskTitle { get; init; }
        public Guid SubTasksId  { get; init; }
        public Guid TaskId  { get; init; }
    }
}
