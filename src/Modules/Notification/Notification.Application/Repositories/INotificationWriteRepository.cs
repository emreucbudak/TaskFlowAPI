namespace Notification.Application.Repositories
{
    public interface INotificationWriteRepository
    {
        Task SendNotification (Notification.Domain.Models.NotificationMessage notification);
        void DeleteNotification (Notification.Domain.Models.NotificationMessage notification);

    }
}
