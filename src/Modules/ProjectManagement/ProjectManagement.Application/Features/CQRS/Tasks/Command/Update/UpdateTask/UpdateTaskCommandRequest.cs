using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask
{
    public class UpdateTaskCommandRequest : IRequest
    {
        public string TaskName { get; set; }
        public string Description { get; set; } 
        public DateTime DeadlineTime { get; set; }
        public Guid TaskId { get; set; }
    }
}
