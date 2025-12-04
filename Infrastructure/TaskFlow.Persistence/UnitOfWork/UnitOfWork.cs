using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.UnitOfWork;
using TaskFlow.Persistence.ApplicationContext;
using TaskFlow.Persistence.Repositories;

namespace TaskFlow.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public IReadRepository<T> GetReadRepository<T>() where T : class, new() => new ReadRepository<T>(_context);

        public IWriteRepository<T> GetWriteRepository<T>() where T : class, new() => new WriteRepository<T>(_context);

        public int SaveChanges()
        {
           return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
