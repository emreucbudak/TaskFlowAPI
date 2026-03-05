using FlashMediator;

namespace Notification.Application.Features.CQRS.Notification.Command.MarkAsRead
{
    public sealed record MarkUserNotificationsAsReadCommandRequest : IRequest<int>
    {
        public MarkUserNotificationsAsReadCommandRequest(Guid userId, int maxCount = 100)
        {
            this.userId = userId;
            MaxCount = maxCount;
        }

        public Guid userId { get; init; }
        public int MaxCount { get; init; }
    }
}
