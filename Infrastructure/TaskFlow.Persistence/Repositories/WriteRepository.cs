using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Repositories;
using TaskFlow.Persistence.ApplicationContext;

namespace TaskFlow.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, new()
    {
        private readonly ApplicationDbContext _context;

        public WriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private DbSet<T> Table => _context.Set<T>();

        public async Task AddAsync(T model)
        {
            await Table.AddAsync(model);
        }

        public Task UpdateAsync(T model)
        {
            Table.Update(model);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T model)
        {
            Table.Remove(model);
            return Task.CompletedTask;
        }
    }
}
