using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageReadRepository
    {
 
        Task<Message> GetByIdAsync(bool trackChanges,Guid id);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid currentUserId,
            Guid userId,
            int pageSize, int page = 1);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid currentUserId,
            Guid userId1,
            Guid userId2,
            int pageSize, int page = 1);
        Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid currentUserId,
            Guid groupId,
            int pageSize, int page = 1);      
        Task<int> GetUnreadMessageCountAsync(string userId);
        Task<IEnumerable<Message>> SearchMessagesAsync(Guid currentUserId,
            string searchTerm,
            int pageSize, int page = 1
            );
        Task<bool> ExistsAsync(Guid id);





    }
}