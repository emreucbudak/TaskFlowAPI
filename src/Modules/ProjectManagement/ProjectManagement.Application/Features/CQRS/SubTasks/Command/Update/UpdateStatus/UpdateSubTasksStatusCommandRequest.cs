using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus
{
    public record UpdateSubTasksStatusCommandRequest : IRequest
    {
        public int TaskStatusId { get; init; }
        public Guid TasksId { get; init; }
    }
}
