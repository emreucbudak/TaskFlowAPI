using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageWriteRepository
    {

        Task<Message> AddAsync(Message message );
        Task<bool> DeleteAsync(Guid id );
        Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids);
        Task<bool> UpdateMessageContentAsync(string id, string newContent);
        Task<bool> MarkAsDeliveredAsync(string messageId );
        Task<bool> DeleteConversationAsync(string userId1, string userId2 );


 

    }
}