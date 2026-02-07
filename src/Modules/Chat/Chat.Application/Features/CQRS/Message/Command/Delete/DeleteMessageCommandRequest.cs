using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.Delete
{
    public record DeleteMessageCommandRequest : IRequest
    {
        public Guid Id { get; init; }

        public DeleteMessageCommandRequest(Guid id)
        {
            Id = id;
        }
    }
}
