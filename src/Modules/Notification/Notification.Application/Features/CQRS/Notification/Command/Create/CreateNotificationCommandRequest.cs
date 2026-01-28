using FlashMediator;

namespace Notification.Application.Features.CQRS.Notification.Command.Create
{
    public record CreateNotificationCommandRequest : IRequest
    {
        public CreateNotificationCommandRequest(string title, string description, DateTime sendTime, bool ısRead, Guid receiverUserId)
        {
            Title = title;
            Description = description;
            SendTime = sendTime;
            IsRead = ısRead;
            ReceiverUserId = receiverUserId;
        }

        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime SendTime { get; init; }
        public bool IsRead { get; init; }
        public Guid ReceiverUserId { get; init; }
    }
}
