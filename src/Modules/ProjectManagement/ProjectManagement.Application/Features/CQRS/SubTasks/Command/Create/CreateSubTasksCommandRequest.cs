using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create
{
    public class CreateSubTasksCommandRequest : IRequest
    {
        public Guid TaskId { get; set; }
        public string Description { get; set; }
        public Guid AssignedUserId  { get; set; }
    }
}
