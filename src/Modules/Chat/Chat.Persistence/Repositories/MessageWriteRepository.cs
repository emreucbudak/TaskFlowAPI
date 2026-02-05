using Chat.Application.Repositories;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chat.Persistence.Repositories
{
    public sealed class MessageWriteRepository(
        ChatDbContext context,
        ILogger<MessageWriteRepository> logger) : IMessageWriteRepository
    {
        public async Task<Message> AddAsync(Message message)
        {
            try
            {
                await context.Messages.AddAsync(message);
                await context.SaveChangesAsync();
                return message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj eklenirken hata oluştu. Gönderen: {SenderId}", message.SenderId);
                throw new Exception("Mesaj gönderilemedi.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var message = await context.Messages.FindAsync(id);
                if (message == null)
                {
                    logger.LogWarning("Silinecek mesaj bulunamadı. ID: {MessageId}", id);
                    return false;
                }

                message.MarkAsDeleted();
                
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj silinirken hata oluştu. ID: {MessageId}", id);
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

                if (messages.Count == 0) return false;

                foreach (var message in messages)
                {
                    message.MarkAsDeleted();
                }
                
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Çoklu mesaj silme işlemi sırasında hata oluştu.");
                throw new Exception("Mesajlar silinemedi.", ex);
            }
        }

        public async Task<bool> UpdateMessageContentAsync(Guid id, string newContent)
        {
            try
            {
                var message = await context.Messages.FindAsync(id);
                if (message == null) return false;

                message.UpdateContent(newContent);
                message.MarkAsEdited();
                
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj içeriği güncellenirken hata oluştu. ID: {MessageId}", id);
                throw new Exception("Mesaj güncellenemedi.", ex);
            }
        }

        public async Task<bool> MarkAsDeliveredAsync(Guid messageId)
        {
            try
            {
                var message = await context.Messages.FindAsync(messageId);
                if (message == null) return false;

                message.MarkAsDelivered();
                
                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj iletildi olarak işaretlenirken hata oluştu. ID: {MessageId}", messageId);
                throw new Exception("İşlem başarısız oldu.", ex);
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

                if (messages.Count == 0) return false;

                foreach (var message in messages)
                {
                    message.MarkAsDeleted();
                }

                var result = await context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{UserId1} ve {UserId2} arasındaki sohbet silinirken hata oluştu.", userId1, userId2);
                throw new Exception("Sohbet silinemedi.", ex);
            }
        }
    }
}