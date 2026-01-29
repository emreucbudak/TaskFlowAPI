using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Persistence.Repositories
{
    public class NotificationWriteRepository(
        NotificationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<NotificationWriteRepository> logger) : INotificationWriteRepository
    {
        private const int MaxNotificationContentLength = 1000;
        private const int MaxNotificationTitleLength = 200;

        /// <summary>
        /// Bildirim gönderir (Validation + Logging + Auto-timestamp)
        /// </summary>
        public async Task SendNotification(NotificationMessage notification)
        {
            // Input validation
            ValidateNotification(notification);

            // ReceiverUserId kontrolü
            if (notification.ReceiverUserId == Guid.Empty)
            {
                logger.LogError("Empty ReceiverUserId in notification");
                throw new ArgumentException(
                    "ReceiverUserId cannot be empty",
                    nameof(notification));
            }

            // Content validation
            if (string.IsNullOrWhiteSpace(notification.Content))
            {
                logger.LogError("Empty notification content");
                throw new ArgumentException(
                    "Notification content cannot be empty",
                    nameof(notification));
            }

            // Content length validation
            if (notification.Content.Length > MaxNotificationContentLength)
            {
                logger.LogWarning(
                    "Notification content too long: {Length} characters, ReceiverUserId: {UserId}",
                    notification.Content.Length,
                    notification.ReceiverUserId);
                throw new ArgumentException(
                    $"Notification content cannot exceed {MaxNotificationContentLength} characters",
                    nameof(notification));
            }

            // Title validation (eğer NotificationMessage'da Title varsa)
            if (!string.IsNullOrEmpty(notification.Title) &&
                notification.Title.Length > MaxNotificationTitleLength)
            {
                logger.LogWarning(
                    "Notification title too long: {Length} characters",
                    notification.Title.Length);
                throw new ArgumentException(
                    $"Notification title cannot exceed {MaxNotificationTitleLength} characters",
                    nameof(notification));
            }

            // Auto-set timestamp (eğer set edilmemişse)
            if (notification.SendTime == default)
            {
                notification.SendTime = DateTime.UtcNow;
            }

            // Initial status
            notification.IsRead = false;

            // Eğer NotificationMessage'da bu alanlar varsa:
            // notification.IsDelivered = false;
            // notification.ReadAt = null;

            logger.LogInformation(
                "Sending notification - ReceiverUserId: {UserId}, Type: {Type}, ContentLength: {Length}",
                notification.ReceiverUserId,
                notification.Type ?? "Unknown",
                notification.Content.Length);

            await context.notificationMessages.AddAsync(notification);

            logger.LogDebug(
                "Notification added to context - Id: {NotificationId}, ReceiverUserId: {UserId}",
                notification.Id,
                notification.ReceiverUserId);
        }

        /// <summary>
        /// Bildirimi siler (Validation + Logging)
        /// NOT: Soft delete yerine hard delete kullanıyor
        /// </summary>
        public void DeleteNotification(NotificationMessage notification)
        {
            // Input validation
            ValidateNotification(notification);

            // ID kontrolü (entity veritabanında mevcut olmalı)
            if (notification.Id == Guid.Empty)
            {
                logger.LogError("Cannot delete notification with empty Id");
                throw new ArgumentException(
                    "Notification Id cannot be empty",
                    nameof(notification));
            }

            logger.LogWarning(
                "Deleting notification - Id: {NotificationId}, ReceiverUserId: {UserId}, Type: {Type}",
                notification.Id,
                notification.ReceiverUserId,
                notification.Type ?? "Unknown");

            // Hard delete
            context.notificationMessages.Remove(notification);

            logger.LogInformation(
                "Notification marked for deletion - Id: {NotificationId}",
                notification.Id);
        }

        /// <summary>
        /// SOFT DELETE versiyonu (Önerilen)
        /// Uncomment edilirse soft delete kullanılabilir
        /// </summary>
        /*
        public void DeleteNotification(NotificationMessage notification)
        {
            ValidateNotification(notification);

            if (notification.Id == Guid.Empty)
            {
                logger.LogError("Cannot delete notification with empty Id");
                throw new ArgumentException(
                    "Notification Id cannot be empty", 
                    nameof(notification));
            }

            // Soft delete (eğer NotificationMessage'da bu alanlar varsa)
            notification.IsDeleted = true;
            notification.DeletedAt = DateTime.UtcNow;
            // notification.DeletedBy = currentUserId; // ICurrentUserService gerekir

            logger.LogWarning(
                "Soft deleting notification - Id: {NotificationId}, ReceiverUserId: {UserId}",
                notification.Id,
                notification.ReceiverUserId);

            context.notificationMessages.Update(notification);

            logger.LogInformation(
                "Notification soft deleted - Id: {NotificationId}",
                notification.Id);
        }
        */

        #region Private Validation Methods

        /// <summary>
        /// Notification entity'sinin null olmadığını kontrol eder
        /// </summary>
        private void ValidateNotification(NotificationMessage notification)
        {
            if (notification == null)
            {
                logger.LogError("Null notification provided to repository");
                throw new ArgumentNullException(
                    nameof(notification),
                    "Notification cannot be null");
            }
        }

        #endregion
    }
}