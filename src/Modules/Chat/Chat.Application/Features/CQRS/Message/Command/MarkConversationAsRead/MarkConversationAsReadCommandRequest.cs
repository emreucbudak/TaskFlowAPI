using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.MarkConversationAsRead
{
    public record MarkConversationAsReadCommandRequest : IRequest<int>
    {
        public Guid CurrentUserId { get; init; }
        public Guid OtherUserId { get; init; }
    }
}
