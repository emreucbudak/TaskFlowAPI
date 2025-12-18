using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus
{
    public class UpdateSubTasksStatusCommandRequest : IRequest
    {
        public int TaskStatusId { get; set; }
    }
}
