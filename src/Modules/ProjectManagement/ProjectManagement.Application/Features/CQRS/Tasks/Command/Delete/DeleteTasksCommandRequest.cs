using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete
{
    public class DeleteTasksCommandRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
