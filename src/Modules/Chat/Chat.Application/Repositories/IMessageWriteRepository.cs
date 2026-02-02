using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    public interface IMessageWriteRepository
    {

        Task<Message> AddAsync(Message message );
        Task<bool> DeleteAsync(Guid id );
        Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids);
        Task<bool> UpdateMessageContentAsync(Guid id, string newContent);
        Task<bool> MarkAsDeliveredAsync(Guid messageId );
        Task<bool> DeleteConversationAsync(Guid userId1, Guid userId2 );


 

    }
}