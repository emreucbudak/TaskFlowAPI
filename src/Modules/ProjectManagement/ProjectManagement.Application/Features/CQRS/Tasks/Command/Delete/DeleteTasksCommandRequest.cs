using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete
{
    public record DeleteTasksCommandRequest : IRequest
    {
        public Guid Id { get; init; }
    }
}
