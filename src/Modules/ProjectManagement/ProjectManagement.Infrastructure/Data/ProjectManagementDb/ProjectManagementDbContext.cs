using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Infrastructure.Data.ProjectManagementDb
{
    public class ProjectManagementDbContext : DbContext
    {
        public ProjectManagementDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ProjectManagementDbContext()
        {
        }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<Domain.Entities.TaskAnswer> TaskAnswers { get; set; }
        public DbSet<Domain.Entities.TaskPriorityCategory> TaskPriorityCategories { get; set; }
        public DbSet<Domain.Entities.TaskStatus> TaskStatuses { get; set; }
        public DbSet<Domain.Entities.Subtask> Subtasks { get; set; }
        public DbSet<Domain.Entities.SubTaskAnswer> SubTaskAnswers { get; set; }
       
    }
}
