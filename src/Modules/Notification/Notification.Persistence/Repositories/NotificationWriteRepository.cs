using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Repositories
{
    public sealed class NotificationWriteRepository(
        NotificationDbContext context,
        ILogger<NotificationWriteRepository> logger) : INotificationWriteRepository
    {
        private const int MaxNotificationContentLength = 1000;
        private const int MaxNotificationTitleLength = 200;

        public async Task SendNotification(NotificationMessage notification)
        {
            ArgumentNullException.ThrowIfNull(notification);

            ValidateReceiverUserId(notification.ReceiverUserId);
            ValidateContent(notification.Description);
            ValidateTitle(notification.Title);
            logger.LogInformation(
                "Bildirim g�nderiliyor - Al�c�: {UserId}, ��erik Uzunlu�u: {Length}",
                notification.ReceiverUserId,
                notification.Description.Length);

            await context.NotificationMessages.AddAsync(notification);

            logger.LogDebug(
                "Bildirim context'e eklendi - Id: {NotificationId}, Al�c�: {UserId}",
                notification.Id,
                notification.ReceiverUserId);
        }

        public void DeleteNotification(NotificationMessage notification)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ValidateId(notification.Id);

            logger.LogWarning(
                "Bildirim siliniyor - Id: {NotificationId}, Al�c�: {UserId}",
                notification.Id,
                notification.ReceiverUserId);

            var entry = context.Entry(notification);

            if (entry.State == EntityState.Detached)
            {
                context.NotificationMessages.Attach(notification);
            }

            context.NotificationMessages.Remove(notification);

            logger.LogInformation(
                "Bildirim silme i�in i�aretlendi - Id: {NotificationId}",
                notification.Id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Bildirim ID'si bo� olamaz", nameof(id));
            }
        }

        private static void ValidateReceiverUserId(Guid receiverUserId)
        {
            if (receiverUserId == Guid.Empty)
            {
                throw new ArgumentException("Al�c� kullan�c� ID'si bo� olamaz", nameof(receiverUserId));
            }
        }

        private static void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Bildirim i�eri�i bo� olamaz", nameof(content));
            }

            if (content.Length > MaxNotificationContentLength)
            {
                throw new ArgumentException(
                    $"Bildirim i�eri�i {MaxNotificationContentLength} karakteri ge�emez",
                    nameof(content));
            }
        }

        private static void ValidateTitle(string? title)
        {
            if (!string.IsNullOrEmpty(title) && title.Length > MaxNotificationTitleLength)
            {
                throw new ArgumentException(
                    $"Bildirim ba�l��� {MaxNotificationTitleLength} karakteri ge�emez",
                    nameof(title));
            }
        }
    }
}

