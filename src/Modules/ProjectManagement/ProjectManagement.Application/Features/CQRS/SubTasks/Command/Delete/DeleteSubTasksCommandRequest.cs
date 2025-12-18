using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public class DeleteSubTasksCommandRequest : IRequest
    {
        public Guid SubTaskId { get; set; }
        public Guid TaskId { get; set; }
    }
}
