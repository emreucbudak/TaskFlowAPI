using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;
using TaskFlow.BuildingBlocks.Common;

namespace Notification.Persistence.Repositories
{
    public sealed class NotificationReadRepository(
        NotificationDbContext context,
        ILogger<NotificationReadRepository> logger) : INotificationReadRepository
    {
        private const int MaxNotificationWindowSize = 100;
        private const int MaxPageSize = MaxNotificationWindowSize;
        private const int DefaultPageSize = 20;

        public async Task<NotificationMessage?> GetByIdAsync(
            bool trackChanges,
            Guid userId,
            Guid notificationId)
        {
            ValidateUserId(userId);
            ValidateNotificationId(notificationId);

            logger.LogInformation(
                "Bildirim getiriliyor - Kullanici: {UserId}, Bildirim: {NotificationId}",
                userId,
                notificationId);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.Id == notificationId && x.ReceiverUserId == userId);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                logger.LogWarning(
                    "Bildirim bulunamadi veya yetkisiz erisim - Kullanici: {UserId}, Bildirim: {NotificationId}",
                    userId,
                    notificationId);
            }
            else
            {
                logger.LogDebug(
                    "Bildirim alindi - Kullanici: {UserId}, Bildirim: {NotificationId}",
                    userId,
                    notificationId);
            }

            return result;
        }

        public async Task<PagedResult<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int pageSize,
            int page = 1,
            bool trackChanges = false)
        {
            ValidateUserId(userId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Kullanici bildirimleri getiriliyor - Kullanici: {UserId}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                userId,
                page,
                pageSize);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId)
                .OrderByDescending(x => x.SendTime)
                .Take(MaxNotificationWindowSize);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var totalCount = await query.CountAsync();

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Kullanici {UserId} icin son {WindowSize} kayittan {Count}/{TotalCount} bildirim getirildi (Sayfa: {Page})",
                userId,
                MaxNotificationWindowSize,
                notifications.Count,
                totalCount,
                page);

            return new PagedResult<NotificationMessage>
            {
                Items = notifications,
                TotalCount = totalCount,
                PageSize = pageSize,
                Page = page
            };
        }

        public async Task<IReadOnlyList<NotificationMessage>> GetUnreadByUserIdAsync(
            Guid userId,
            int maxCount,
            bool trackChanges = false)
        {
            ValidateUserId(userId);

            var normalizedCount = NormalizeWindowCount(maxCount);

            logger.LogInformation(
                "Kullanici okunmamis bildirimleri getiriliyor - Kullanici: {UserId}, Limit: {Limit}",
                userId,
                normalizedCount);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId && !x.IsRead)
                .OrderByDescending(x => x.SendTime)
                .Take(normalizedCount);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var unreadNotifications = await query.ToListAsync();

            logger.LogInformation(
                "Kullanici {UserId} icin {Count} okunmamis bildirim bulundu (Limit: {Limit})",
                userId,
                unreadNotifications.Count,
                normalizedCount);

            return unreadNotifications;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            ValidateUserId(userId);

            logger.LogInformation("Kullanici {UserId} icin okunmamis bildirim sayisi hesaplaniyor", userId);

            var count = await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x => x.ReceiverUserId == userId && !x.IsRead);

            logger.LogInformation(
                "Kullanici {UserId} icin okunmamis bildirim sayisi: {Count}",
                userId,
                count);

            return count;
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Kullanici ID'si bos olamaz", nameof(userId));
            }
        }

        private static void ValidateNotificationId(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                throw new ArgumentException("Bildirim ID'si bos olamaz", nameof(notificationId));
            }
        }

        private static int NormalizeWindowCount(int maxCount)
        {
            if (maxCount < 1)
            {
                return MaxNotificationWindowSize;
            }

            return Math.Min(maxCount, MaxNotificationWindowSize);
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
