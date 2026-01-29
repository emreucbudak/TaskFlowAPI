using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageReadRepository
    {
 
        Task<Message> GetByIdAsync(bool trackChanges,Guid id);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2);
        Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(Guid groupId);      
        Task<int> GetUnreadMessageCountAsync(string userId);
        Task<IEnumerable<Message>> SearchMessagesAsync(string searchTerm);
        Task<bool> ExistsAsync(Guid id);





    }
}