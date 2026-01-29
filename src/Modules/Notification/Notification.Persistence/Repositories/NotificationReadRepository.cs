using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Data.NotificationDb;

namespace Notification.Persistence.Repositories
{
    public class NotificationReadRepository(
        NotificationDbContext context,
        ILogger<NotificationReadRepository> logger,
        IMemoryCache cache) : INotificationReadRepository
    {
        private const int MaxPageSize = 200;
        private const int DefaultPageSize = 20;
        private const int CacheDurationSeconds = 30;

        /// <summary>
        /// Belirli bir bildirimi ID'ye göre getirir (Authorization kontrollü)
        /// </summary>
        public async Task<NotificationMessage?> GetByIdAsync(
            bool trackChanges,
            Guid userId,
            Guid notificationId)
        {
            // Input validation
            ValidateUserId(userId);
            ValidateNotificationId(notificationId);

            logger.LogInformation(
                "Fetching notification - User: {UserId}, NotificationId: {NotificationId}",
                userId, notificationId);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x =>
                    x.Id == notificationId &&
                    x.ReceiverUserId == userId);

            // Soft delete filtresi (eğer NotificationMessage'da IsDeleted property'si varsa)
            // query = query.Where(x => !x.IsDeleted);

            if (!trackChanges)
                query = query.AsNoTracking();

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                logger.LogWarning(
                    "Notification not found or unauthorized - User: {UserId}, NotificationId: {NotificationId}",
                    userId, notificationId);

                // Anti-enumeration: Timing attack prevention (opsiyonel)
                // await Task.Delay(Random.Shared.Next(5, 15));
            }
            else
            {
                logger.LogDebug(
                    "Notification retrieved - User: {UserId}, NotificationId: {NotificationId}",
                    userId, notificationId);
            }

            return result;
        }

        /// <summary>
        /// Kullanıcının bildirimlerini sayfalı olarak getirir (GÜVENLİ VERSİYON)
        /// </summary>
        public async Task<List<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            // Input validation
            ValidateUserId(userId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Fetching user notifications - User: {UserId}, Page: {Page}, PageSize: {PageSize}",
                userId, page, pageSize);

            IQueryable<NotificationMessage> query = context.notificationMessages
                .Where(x => x.ReceiverUserId == userId)
                .OrderByDescending(x => x.SendTime);

            // Soft delete filtresi
            // query = query.Where(x => !x.IsDeleted);

            if (!trackChanges)
                query = query.AsNoTracking();

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {Count} notifications for user {UserId}",
                notifications.Count, userId);

            return notifications;
        }

        /// <summary>
        /// BACKWARD COMPATIBILITY: Eski take parametreli versiyon (Deprecated)
        /// </summary>
        [Obsolete("Use GetByUserIdAsync(userId, page, pageSize) instead")]
        public async Task<List<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int take,
            bool trackChanges = false)
        {
            // Eski API'yi yeni API'ye yönlendir
            // take parametresini pageSize olarak kullan
            var pageSize = take;
            ValidatePagination(ref pageSize);

            return await GetByUserIdAsync(userId, 1, pageSize, trackChanges);
        }

        /// <summary>
        /// Kullanıcının okunmamış bildirim sayısını getirir (Cache'li)
        /// </summary>
        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            // Input validation
            ValidateUserId(userId);

            var cacheKey = $"unread_count_{userId}";

            // Cache kontrolü
            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                logger.LogDebug(
                    "Returning cached unread count for user {UserId}: {Count}",
                    userId, cachedCount);
                return cachedCount;
            }

            logger.LogInformation(
                "Calculating unread count for user {UserId}",
                userId);

            var count = await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x =>
                    x.ReceiverUserId == userId &&
                    !x.IsRead);
            // && !x.IsDeleted); // Soft delete varsa

            // Cache'e kaydet
            cache.Set(cacheKey, count, TimeSpan.FromSeconds(CacheDurationSeconds));

            logger.LogInformation(
                "Unread count for user {UserId}: {Count}",
                userId, count);

            return count;
        }

        /// <summary>
        /// Kullanıcının tüm bildirimlerinin sayısını getirir
        /// </summary>
        public async Task<int> GetTotalCountAsync(Guid userId)
        {
            ValidateUserId(userId);

            logger.LogDebug("Getting total notification count for user {UserId}", userId);

            var count = await context.notificationMessages
                .AsNoTracking()
                .CountAsync(x => x.ReceiverUserId == userId);
            // && !x.IsDeleted); // Soft delete varsa

            return count;
        }

        /// <summary>
        /// Cache'i temizle (notification okunduğunda çağrılmalı)
        /// </summary>
        public void InvalidateUnreadCountCache(Guid userId)
        {
            var cacheKey = $"unread_count_{userId}";
            cache.Remove(cacheKey);

            logger.LogDebug("Invalidated unread count cache for user {UserId}", userId);
        }

        #region Private Validation Methods

        private void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                logger.LogError("Empty userId provided");
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            }
        }

        private void ValidateNotificationId(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                logger.LogError("Empty notificationId provided");
                throw new ArgumentException("NotificationId cannot be empty", nameof(notificationId));
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning("Invalid page number: {Page}, resetting to 1", page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning(
                    "Invalid page size: {PageSize}, resetting to default {DefaultPageSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Page size {PageSize} exceeds maximum {MaxPageSize}, limiting",
                    pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidatePagination(ref int pageSize)
        {
            if (pageSize < 1)
                pageSize = DefaultPageSize;

            if (pageSize > MaxPageSize)
                pageSize = MaxPageSize;
        }

        #endregion
    }
}