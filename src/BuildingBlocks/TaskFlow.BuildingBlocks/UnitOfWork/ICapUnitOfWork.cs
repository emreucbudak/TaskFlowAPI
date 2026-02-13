using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;

namespace TaskFlow.BuildingBlocks.UnitOfWork
{
    public interface ICapUnitOfWork : IUnitOfWork
    {
        IDbContextTransaction BeginTransaction(ICapPublisher publisher, bool autoCommit = false);
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
