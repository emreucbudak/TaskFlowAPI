using FlashMediator;

namespace Notification.Application.Features.CQRS.Notification.Command.Delete
{
    public record DeleteNotificationCommandRequest : IRequest
    {
        public DeleteNotificationCommandRequest(Guid userId, Guid notificationId)
        {
            this.userId = userId;
            this.notificationId = notificationId;
        }

        public Guid userId { get; init; }
        public Guid notificationId { get; init; }
    }
}
