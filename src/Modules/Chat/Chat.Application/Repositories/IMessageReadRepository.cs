using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageReadRepository
    {
 
        Task<Message> GetByIdAsync(bool trackChanges,Guid id);
        Task<IEnumerable<Message>> GetMessagesByUserIdAsync(Guid userId);

        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(string userId1, string userId2, CancellationToken cancellationToken = default);


        Task<IEnumerable<Message>> GetMessagesByGroupIdAsync(string groupId);

   
        Task<IEnumerable<Message>> GetMessagesPagedAsync(int skip, int take);

       
        Task<IEnumerable<Message>> GetMessagesSinceDateAsync(DateTime date);

        Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate);


        Task<IEnumerable<Message>> GetUnreadMessagesByUserIdAsync(Guid userId);

        Task<int> GetUnreadMessageCountAsync(string userId);

  
        Task<IEnumerable<Message>> GetLastMessagesAsync(int count);

        Task<IEnumerable<Message>> GetLastGroupMessagesAsync(string groupId, int count);


        Task<IEnumerable<Message>> GetLastMessagesBetweenUsersAsync(string userId1, string userId2, int count);


        Task<IEnumerable<Message>> SearchMessagesAsync(string searchTerm);

       
        Task<IEnumerable<Message>> SearchMessagesByUserAsync(string userId, string searchTerm);


        Task<bool> ExistsAsync(string id);





    }
}