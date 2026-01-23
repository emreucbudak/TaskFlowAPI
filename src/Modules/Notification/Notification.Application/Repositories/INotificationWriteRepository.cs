namespace Notification.Application.Repositories
{
    public interface INotificationWriteRepository
    {
        Task SendNotification (Notification.Domain.Models.NotificationMessage notification);

    }
}
