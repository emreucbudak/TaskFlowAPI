using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageWriteRepository
    {

        Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);


        Task<IEnumerable<Message>> AddRangeAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);

 
        Task<Message> UpdateAsync(Message message, CancellationToken cancellationToken = default);

  
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);


        Task<bool> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);


        Task<bool> SoftDeleteAsync(string id, CancellationToken cancellationToken = default);


        Task<bool> UpdateMessageContentAsync(string id, string newContent, CancellationToken cancellationToken = default);


        Task<bool> MarkAsReadAsync(string messageId, string userId, CancellationToken cancellationToken = default);


        Task<bool> MarkMultipleAsReadAsync(IEnumerable<string> messageIds, string userId, CancellationToken cancellationToken = default);


        Task<bool> MarkAllAsReadByUserAsync(string userId, CancellationToken cancellationToken = default);


        Task<bool> MarkConversationAsReadAsync(string userId1, string userId2, CancellationToken cancellationToken = default);


        Task<bool> AddReactionAsync(string messageId, string userId, string reaction, CancellationToken cancellationToken = default);


        Task<bool> RemoveReactionAsync(string messageId, string userId, string reaction, CancellationToken cancellationToken = default);

    
        Task<bool> MarkAsSentAsync(string messageId, CancellationToken cancellationToken = default);

     
        Task<bool> MarkAsDeliveredAsync(string messageId, CancellationToken cancellationToken = default);

 
        Task<bool> DeleteAllMessagesByUserAsync(string userId, CancellationToken cancellationToken = default);


        Task<bool> DeleteAllMessagesByGroupAsync(string groupId, CancellationToken cancellationToken = default);


        Task<bool> DeleteConversationAsync(string userId1, string userId2, CancellationToken cancellationToken = default);

       
        Task<bool> DeleteMessagesOlderThanAsync(DateTime date, CancellationToken cancellationToken = default);


        Task<bool> AddEditHistoryAsync(string messageId, string oldContent, CancellationToken cancellationToken = default);


        Task<bool> PinMessageAsync(string messageId, CancellationToken cancellationToken = default);


        Task<bool> UnpinMessageAsync(string messageId, CancellationToken cancellationToken = default);

 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}