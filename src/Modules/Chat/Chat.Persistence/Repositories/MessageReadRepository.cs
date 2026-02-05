using Chat.Application.Repositories;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Persistence.Repositories
{
    public sealed class MessageReadRepository(
        ChatDbContext context,
        ILogger<MessageReadRepository> logger) : IMessageReadRepository
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        public async Task<Message> GetByIdAsync(bool trackChanges, Guid id)
        {
            try
            {
                var query = context.Messages.AsQueryable();
                if (!trackChanges)
                    query = query.AsNoTracking();

                var message = await query.FirstOrDefaultAsync(m => m.Id == id);
                
                if (message == null)
                    logger.LogWarning("Mesaj bulunamadı. ID: {MessageId}", id);

                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{MessageId} ID'li mesaj getirilirken bir hata oluştu.", id);
                throw new Exception($"{id} ID'li mesaj getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                return await context.Messages
                    .AsNoTracking()
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId} kullanıcısının mesajları getirilirken bir hata oluştu.", userId);
                throw new Exception($"{userId} kullanıcısının mesajları getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid currentUserId, Guid userId1, Guid userId2, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                if (currentUserId != userId1 && currentUserId != userId2)
                {
                    logger.LogWarning("Yetkisiz mesaj erişim denemesi. İsteyen: {CurrentUserId}, Hedefler: {UserId1}, {UserId2}", currentUserId, userId1, userId2);
                    return [];
                }

                return await context.Messages
                    .AsNoTracking()
                    .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || 
                                (m.SenderId == userId2 && m.ReceiverId == userId1))
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId1} ve {UserId2} kullanıcıları arasındaki mesajlar getirilirken bir hata oluştu.", userId1, userId2);
                throw new Exception("Mesaj geçmişi getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid currentUserId, Guid groupId, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                return await context.Messages
                    .AsNoTracking()
                    .Where(m => m.GroupId == groupId)
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{GroupId} grubunun mesajları getirilirken bir hata oluştu.", groupId);
                throw new Exception("Grup mesajları getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId)
        {
            try
            {
                return await context.Messages
                    .AsNoTracking()
                    .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId} kullanıcısı için okunmamış mesaj sayısı alınırken hata oluştu.", userId);
                throw new Exception("Okunmamış mesaj sayısı alınamadı.", ex);
            }
        }

        public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid currentUserId, string searchTerm, int pageSize, int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return [];

                ValidatePagination(ref page, ref pageSize);

                return await context.Messages
                    .AsNoTracking()
                    .Where(m => (m.SenderId == currentUserId || m.ReceiverId == currentUserId) && 
                                EF.Functions.Like(m.Content, $"%{searchTerm}%"))
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId} kullanıcısının mesaj araması sırasında hata oluştu. Terim: {SearchTerm}", currentUserId, searchTerm);
                throw new Exception("Mesaj arama işlemi başarısız oldu.", ex);
            }
        }

        private static void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        }
    }
}