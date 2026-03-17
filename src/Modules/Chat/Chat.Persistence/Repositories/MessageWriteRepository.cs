using Chat.Application.Repositories;
using Chat.Application.Services;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Persistence.Repositories
{
    public sealed class MessageWriteRepository(
        ChatDbContext context,
        IChatMessageEncryptionService messageEncryptionService,
        ILogger<MessageWriteRepository> logger) : IMessageWriteRepository
    {
        public async Task<Message> AddAsync(Message message)
        {
            try
            {
                message.UpdateContent(messageEncryptionService.Encrypt(message.Content));
                await context.Messages.AddAsync(message);
                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj eklenirken hata olustu. Gonderen: {SenderId}", message.SenderId);
                throw new Exception("Mesaj gonderilemedi.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var message = await context.Messages.FindAsync(id);
                if (message == null)
                {
                    logger.LogWarning("Silinecek mesaj bulunamadi. ID: {MessageId}", id);
                    return false;
                }

                message.MarkAsDeleted();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj silinirken hata olustu. ID: {MessageId}", id);
                throw new Exception("Mesaj silinemedi.", ex);
            }
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var messages = await context.Messages
                    .Where(m => ids.Contains(m.Id))
                    .ToListAsync();

                if (messages.Count == 0)
                {
                    return false;
                }

                foreach (var message in messages)
                {
                    message.MarkAsDeleted();
                }

                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Coklu mesaj silme islemi sirasinda hata olustu.");
                throw new Exception("Mesajlar silinemedi.", ex);
            }
        }

        public async Task<bool> UpdateMessageContentAsync(Guid id, string newContent)
        {
            try
            {
                var message = await context.Messages.FindAsync(id);
                if (message == null)
                {
                    return false;
                }

                message.UpdateContent(messageEncryptionService.Encrypt(newContent));
                message.MarkAsEdited();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj icerigi guncellenirken hata olustu. ID: {MessageId}", id);
                throw new Exception("Mesaj guncellenemedi.", ex);
            }
        }

        public async Task<bool> MarkAsDeliveredAsync(Guid messageId)
        {
            try
            {
                var message = await context.Messages.FindAsync(messageId);
                if (message == null)
                {
                    return false;
                }

                message.MarkAsDelivered();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj iletildi olarak isaretlenirken hata olustu. ID: {MessageId}", messageId);
                throw new Exception("Islem basarisiz oldu.", ex);
            }
        }

        public async Task<int> MarkConversationAsReadAsync(Guid currentUserId, Guid otherUserId)
        {
            try
            {
                var unreadMessages = await context.Messages
                    .Where(message =>
                        !message.IsDeleted &&
                        message.GroupId == null &&
                        message.ReceiverId == currentUserId &&
                        message.SenderId == otherUserId &&
                        !message.IsRead)
                    .ToListAsync();

                if (unreadMessages.Count == 0)
                {
                    return 0;
                }

                foreach (var message in unreadMessages)
                {
                    message.MarkAsRead(true);
                }

                return unreadMessages.Count;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sohbet mesajlari okundu olarak isaretlenirken hata olustu. UserId: {CurrentUserId}, OtherUserId: {OtherUserId}", currentUserId, otherUserId);
                throw new Exception("Sohbet okundu olarak isaretlenemedi.", ex);
            }
        }

        public async Task<bool> DeleteConversationAsync(Guid userId1, Guid userId2)
        {
            try
            {
                var messages = await context.Messages
                    .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                                (m.SenderId == userId2 && m.ReceiverId == userId1))
                    .ToListAsync();

                if (messages.Count == 0)
                {
                    return false;
                }

                foreach (var message in messages)
                {
                    message.MarkAsDeleted();
                }

                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId1} ve {UserId2} arasindaki sohbet silinirken hata olustu.", userId1, userId2);
                throw new Exception("Sohbet silinemedi.", ex);
            }
        }
    }
}
