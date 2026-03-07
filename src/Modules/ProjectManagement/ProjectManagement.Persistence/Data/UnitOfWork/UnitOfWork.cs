using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectManagement.Persistence.Data.ProjectManagementDb;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Persistence.Data.UnitOfWork
{
    public class UnitOfWork(ProjectManagementDbContext context) : IProjectManagementCapUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public IDbContextTransaction BeginTransaction(ICapPublisher publisher, bool autoCommit = false)
        {
            _currentTransaction = context.Database.BeginTransaction(publisher, autoCommit);
            return _currentTransaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await context.SaveChangesAsync(cancellationToken);

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Rollback()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            context.Dispose();
            _currentTransaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
