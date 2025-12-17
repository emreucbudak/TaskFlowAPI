using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementWriteRepository : IProjectManagementWriteRepository
    {
        private readonly ProjectManagementDbContext _context;

        public ProjectManagementWriteRepository(ProjectManagementDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task AddTask(Domain.Entities.Task task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public void DeleteTask(Domain.Entities.Task task)
        {
            _context.Tasks.Remove(task);
        }

        public void UpdateTask(Domain.Entities.Task task)
        {
            _context.Tasks.Update(task);
        }
    }
}
