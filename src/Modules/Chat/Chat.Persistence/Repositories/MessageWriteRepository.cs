using Chat.Application.Repositories;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;

namespace Chat.Persistence.Repositories
{
    public class MessageWriteRepository(ChatDbContext context) : IMessageWriteRepository
    {
        public async Task<Message> AddAsync(Message message)
        {
            await context.Messages.AddAsync(message);
            return message;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var message = await context.Messages.FindAsync(id);
            if (message == null) return false;
            context.Messages.Remove(message);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            var messages = await context.Messages.Where(m => ids.Contains(m.Id)).ToListAsync();
            if (!messages.Any()) return false;
            context.Messages.RemoveRange(messages);
            return true;
        }

        public async Task<bool> UpdateMessageContentAsync(Guid id, string newContent)
        {
            var message = await context.Messages.FindAsync(id);
            if (message == null) return false;
            message.UpdateContent(newContent);
            message.MarkAsEdited();
            context.Messages.Update(message);
            return true;
        }

        public async Task<bool> MarkAsDeliveredAsync(Guid messageId)
        {
            var message = await context.Messages.FindAsync(messageId);
            if (message == null) return false;
            message.MarkAsDelivered();
            context.Messages.Update(message);
            return true;
        }

        public async Task<bool> DeleteConversationAsync(Guid userId1, Guid userId2)
        {
            var messages = await context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || (m.SenderId == userId2 && m.ReceiverId == userId1))
                .ToListAsync();
            if (!messages.Any()) return false;
            context.Messages.RemoveRange(messages);
            return true;
        }
    }
}