using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Repositories;

namespace TaskFlow.Application.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IReadRepository<T> GetReadRepository<T>() where T : class, new();
        public IWriteRepository<T> GetWriteRepository<T>() where T : class, new();
        public Task<int> SaveChangesAsync();
        public int SaveChanges();
    }
}
