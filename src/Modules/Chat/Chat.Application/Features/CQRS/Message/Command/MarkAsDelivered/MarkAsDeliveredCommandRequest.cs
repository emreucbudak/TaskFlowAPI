using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.MarkAsDelivered
{
    public record MarkAsDeliveredCommandRequest : IRequest
    {
        public Guid MessageId { get; init; }

        public MarkAsDeliveredCommandRequest(Guid messageId)
        {
            MessageId = messageId;
        }
    }
}
