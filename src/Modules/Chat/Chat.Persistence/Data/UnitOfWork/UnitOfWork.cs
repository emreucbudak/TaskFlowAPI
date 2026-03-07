using Chat.Persistence.Data.ChatDb;
using Chat.Application.UnitOfWork;

namespace Chat.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ChatDbContext context) : IChatUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
