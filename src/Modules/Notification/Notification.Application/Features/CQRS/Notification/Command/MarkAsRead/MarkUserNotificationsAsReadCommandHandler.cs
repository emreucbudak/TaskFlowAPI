using FlashMediator;
using Notification.Application.Repositories;
using Notification.Application.UnitOfWork;

namespace Notification.Application.Features.CQRS.Notification.Command.MarkAsRead
{
    public sealed class MarkUserNotificationsAsReadCommandHandler(
        INotificationReadRepository readRepository,
        INotificationUnitOfWork unitOfWork) : IRequestHandler<MarkUserNotificationsAsReadCommandRequest, int>
    {
        public async Task<int> Handle(MarkUserNotificationsAsReadCommandRequest request, CancellationToken cancellationToken)
        {
            var notifications = await readRepository.GetUnreadByUserIdAsync(
                request.userId,
                request.MaxCount,
                trackChanges: true);

            var updatedCount = 0;
            foreach (var notification in notifications)
            {
                if (notification.IsRead)
                {
                    continue;
                }

                notification.MarkAsRead();
                updatedCount++;
            }

            if (updatedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return updatedCount;
        }
    }
}

