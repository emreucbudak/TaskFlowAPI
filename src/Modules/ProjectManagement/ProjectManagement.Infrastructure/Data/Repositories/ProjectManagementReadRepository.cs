using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementReadRepository : IProjectManagementReadRepository
    {
        private readonly ProjectManagementDbContext _context;

        public ProjectManagementReadRepository(ProjectManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.Task>> GetAllTasks(bool trackChanges)
        {
            IQueryable<Domain.Entities.Task> query = _context.Tasks;
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<Domain.Entities.Task> GetTask(Guid id, bool trackChanges)
        {
            IQueryable<Domain.Entities.Task> query = _context.Tasks;
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
