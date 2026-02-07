using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.Update
{
    public record UpdateMessageCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public string NewContent { get; init; }

        public UpdateMessageCommandRequest(Guid id, string newContent)
        {
            Id = id;
            NewContent = newContent;
        }
    }
}
