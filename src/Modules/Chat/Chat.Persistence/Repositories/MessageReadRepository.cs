using Chat.Application.Repositories;
using Chat.Domain.Entities;
using Chat.Persistence.Data.ChatDb;
using Microsoft.EntityFrameworkCore;

namespace Chat.Persistence.Repositories
{
    public class MessageReadRepository(ChatDbContext context) : IMessageReadRepository
    {
        public async Task<Message?> GetByIdAsync(bool trackChanges, Guid id)
        {
            var query = trackChanges ? context.Messages : context.Messages.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId, int pageSize, int page = 1)
        {
            return await context.Messages
                .AsNoTracking()
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SendTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid currentUserId, Guid userId1, Guid userId2, int pageSize, int page = 1)
        {
            return await context.Messages
                .AsNoTracking()
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SendTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid currentUserId, Guid groupId, int pageSize, int page = 1)
        {
            return await context.Messages
                .AsNoTracking()
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.SendTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid userId)
        {
            return await context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid currentUserId, string searchTerm, int pageSize, int page = 1)
        {
            return await context.Messages
                .AsNoTracking()
                .Where(m => (m.SenderId == currentUserId || m.ReceiverId == currentUserId) && m.Content.Contains(searchTerm))
                .OrderByDescending(m => m.SendTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
