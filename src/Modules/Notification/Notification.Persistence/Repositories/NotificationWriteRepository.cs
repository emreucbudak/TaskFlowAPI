using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Persistence.Repositories
{
    public sealed class NotificationWriteRepository(
        NotificationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<NotificationWriteRepository> logger) : INotificationWriteRepository
    {
        private const int MaxNotificationContentLength = 1000;
        private const int MaxNotificationTitleLength = 200;

        public async Task SendNotification(NotificationMessage notification)
        {
            ArgumentNullException.ThrowIfNull(notification);

            ValidateReceiverUserId(notification.ReceiverUserId);
            ValidateContent(notification.Content);
            ValidateTitle(notification.Title);

            if (notification.SendTime == default)
            {
                notification.SendTime = DateTime.UtcNow;
            }

            notification.IsRead = false;

            logger.LogInformation(
                "Bildirim gönderiliyor - Alıcı: {UserId}, Tip: {Type}, İçerik Uzunluğu: {Length}",
                notification.ReceiverUserId,
                notification.Type ?? "Bilinmiyor",
                notification.Content.Length);

            await context.notificationMessages.AddAsync(notification);

            logger.LogDebug(
                "Bildirim context'e eklendi - Id: {NotificationId}, Alıcı: {UserId}",
                notification.Id,
                notification.ReceiverUserId);
        }

        public void DeleteNotification(NotificationMessage notification)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ValidateId(notification.Id);

            logger.LogWarning(
                "Bildirim siliniyor - Id: {NotificationId}, Alıcı: {UserId}, Tip: {Type}",
                notification.Id,
                notification.ReceiverUserId,
                notification.Type ?? "Bilinmiyor");

            var entry = context.Entry(notification);

            if (entry.State == EntityState.Detached)
            {
                context.notificationMessages.Attach(notification);
            }

            context.notificationMessages.Remove(notification);

            logger.LogInformation(
                "Bildirim silme için işaretlendi - Id: {NotificationId}",
                notification.Id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Bildirim ID'si boş olamaz", nameof(id));
            }
        }

        private static void ValidateReceiverUserId(Guid receiverUserId)
        {
            if (receiverUserId == Guid.Empty)
            {
                throw new ArgumentException("Alıcı kullanıcı ID'si boş olamaz", nameof(receiverUserId));
            }
        }

        private static void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Bildirim içeriği boş olamaz", nameof(content));
            }

            if (content.Length > MaxNotificationContentLength)
            {
                throw new ArgumentException(
                    $"Bildirim içeriği {MaxNotificationContentLength} karakteri geçemez",
                    nameof(content));
            }
        }

        private static void ValidateTitle(string? title)
        {
            if (!string.IsNullOrEmpty(title) && title.Length > MaxNotificationTitleLength)
            {
                throw new ArgumentException(
                    $"Bildirim başlığı {MaxNotificationTitleLength} karakteri geçemez",
                    nameof(title));
            }
        }
    }
}