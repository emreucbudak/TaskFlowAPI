using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageReadRepository
    {
 
        Task<Message> GetByIdAsync(bool trackChanges,Guid id);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId,
            int pageSize, int page = 1);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid currentUserId,
            Guid userId1,
            Guid userId2,
            int pageSize, int page = 1);
        Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid currentUserId,
            Guid groupId,
            int pageSize, int page = 1);      
        Task<int> GetUnreadMessageCountAsync(Guid userId);
        Task<IEnumerable<Message>> SearchMessagesAsync(Guid currentUserId,
            string searchTerm,
            int pageSize, int page = 1
            );






    }
}