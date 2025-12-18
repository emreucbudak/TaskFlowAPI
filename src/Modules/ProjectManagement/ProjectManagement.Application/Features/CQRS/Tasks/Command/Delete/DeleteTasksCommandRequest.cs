using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete
{
    public record DeleteTasksCommandRequest : IRequest
    {
        public Guid Id { get; init; }
    }
}
