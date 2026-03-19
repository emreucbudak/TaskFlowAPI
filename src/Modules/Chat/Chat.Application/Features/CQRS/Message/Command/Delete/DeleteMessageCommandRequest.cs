using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.Delete
{
    public record DeleteMessageCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public Guid CurrentUserId { get; init; }

        public DeleteMessageCommandRequest(Guid id, Guid currentUserId = default)
        {
            Id = id;
            CurrentUserId = currentUserId;
        }
    }
}
