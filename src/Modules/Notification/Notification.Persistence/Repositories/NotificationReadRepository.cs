using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Repositories
{
    public sealed class NotificationReadRepository(
        NotificationDbContext context,
        ILogger<NotificationReadRepository> logger) : INotificationReadRepository
    {
        private const int MaxPageSize = 200;
        private const int DefaultPageSize = 20;

        public async Task<NotificationMessage?> GetByIdAsync(
            bool trackChanges,
            Guid userId,
            Guid notificationId)
        {
            ValidateUserId(userId);
            ValidateNotificationId(notificationId);

            logger.LogInformation(
                "Bildirim getiriliyor - Kullanıcı: {UserId}, Bildirim: {NotificationId}",
                userId, notificationId);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.Id == notificationId && x.ReceiverUserId == userId);

            if (!trackChanges)
                query = query.AsNoTracking();

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                logger.LogWarning(
                    "Bildirim bulunamadı veya yetkisiz erişim - Kullanıcı: {UserId}, Bildirim: {NotificationId}",
                    userId, notificationId);
            }
            else
            {
                logger.LogDebug(
                    "Bildirim alındı - Kullanıcı: {UserId}, Bildirim: {NotificationId}",
                    userId, notificationId);
            }

            return result;
        }

        public async Task<List<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int pageSize,
            int page = 1,
            bool trackChanges = false)
        {
            ValidateUserId(userId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Kullanıcı bildirimleri getiriliyor - Kullanıcı: {UserId}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                userId, page, pageSize);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId)
                .OrderByDescending(x => x.SendTime);

            if (!trackChanges)
                query = query.AsNoTracking();

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Kullanıcı {UserId} için {Count} adet bildirim getirildi",
                userId, notifications.Count);

            return notifications;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            ValidateUserId(userId);

            logger.LogInformation("Kullanıcı {UserId} için okunmamış bildirim sayısı hesaplanıyor", userId);

            var count = await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x => x.ReceiverUserId == userId && !x.IsRead);

            logger.LogInformation(
                "Kullanıcı {UserId} için okunmamış bildirim sayısı: {Count}",
                userId, count);

            return count;
        }

        public async Task<int> GetTotalCountAsync(Guid userId)
        {
            ValidateUserId(userId);

            logger.LogDebug("Kullanıcı {UserId} için toplam bildirim sayısı alınıyor", userId);

            var count = await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x => x.ReceiverUserId == userId);

            return count;
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Kullanıcı ID'si boş olamaz", nameof(userId));
            }
        }

        private static void ValidateNotificationId(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                throw new ArgumentException("Bildirim ID'si boş olamaz", nameof(notificationId));
            }
        }

        private static void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                pageSize = MaxPageSize;
            }
        }


    }
}