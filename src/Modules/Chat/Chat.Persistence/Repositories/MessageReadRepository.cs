using Chat.Application.Repositories;
using Chat.Application.Services;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Chat.Persistence.Repositories
{
    public sealed class MessageReadRepository(
        ChatDbContext context,
        IChatMessageEncryptionService messageEncryptionService,
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
                {
                    query = query.AsNoTracking();
                }

                var message = await query.FirstOrDefaultAsync(m => m.Id == id);
                if (message == null)
                {
                    logger.LogWarning("Message not found. ID: {MessageId}", id);
                    return null!;
                }

                if (!trackChanges)
                {
                    DecryptMessageContent(message);
                }

                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get message by id. ID: {MessageId}", id);
                throw new Exception($"Failed to get message by id: {id}", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                var messages = await context.Messages
                    .AsNoTracking()
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return DecryptMessageContents(messages);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get messages by user. UserId: {UserId}", userId);
                throw new Exception($"Failed to get messages by user: {userId}", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid currentUserId, Guid userId1, Guid userId2, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                if (currentUserId != userId1 && currentUserId != userId2)
                {
                    logger.LogWarning("Unauthorized message access attempt. Requester: {CurrentUserId}, Targets: {UserId1}, {UserId2}", currentUserId, userId1, userId2);
                    return [];
                }

                var messages = await context.Messages
                    .AsNoTracking()
                    .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                                (m.SenderId == userId2 && m.ReceiverId == userId1))
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return DecryptMessageContents(messages);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get messages between users. UserId1: {UserId1}, UserId2: {UserId2}", userId1, userId2);
                throw new Exception("Failed to get conversation messages.", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid currentUserId, Guid groupId, int pageSize, int page = 1)
        {
            try
            {
                ValidatePagination(ref page, ref pageSize);

                var messages = await context.Messages
                    .AsNoTracking()
                    .Where(m => m.GroupId == groupId)
                    .OrderByDescending(m => m.SendTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return DecryptMessageContents(messages);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get messages by group. GroupId: {GroupId}", groupId);
                throw new Exception("Failed to get group messages.", ex);
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
                logger.LogError(ex, "Failed to get unread message count. UserId: {UserId}", userId);
                throw new Exception("Failed to get unread message count.", ex);
            }
        }

        public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid currentUserId, string searchTerm, int pageSize, int page = 1)
        {
            await Task.CompletedTask;
            return [];
        }

        private static void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        }

        private IEnumerable<Message> DecryptMessageContents(IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                DecryptMessageContent(message);
            }

            return messages;
        }

        private void DecryptMessageContent(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return;
            }

            try
            {
                message.UpdateContent(messageEncryptionService.Decrypt(message.Content));
            }
            catch (CryptographicException ex)
            {
                logger.LogError(ex, "Failed to decrypt message content. MessageId: {MessageId}", message.Id);
                message.UpdateContent("[message-unavailable]");
            }
            catch (FormatException ex)
            {
                logger.LogError(ex, "Invalid encrypted payload format. MessageId: {MessageId}", message.Id);
                message.UpdateContent("[message-unavailable]");
            }
        }
    }
}
