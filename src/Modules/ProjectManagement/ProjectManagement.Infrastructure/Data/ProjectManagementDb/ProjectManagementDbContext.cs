using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Data.DataConfiguration;

namespace ProjectManagement.Infrastructure.Data.ProjectManagementDb
{
    public class ProjectManagementDbContext : DbContext
    {
        public ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options) : base(options)
        {
        }

        protected ProjectManagementDbContext()
        {
        }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<Domain.Entities.TaskPriorityCategory> TaskPriorityCategories { get; set; }
        public DbSet<Domain.Entities.TaskStatus> TaskStatuses { get; set; }
        public DbSet<Domain.Entities.Subtask> Subtasks { get; set; }
        public DbSet<Domain.Entities.IndividualTasks> IndividualTasks { get; set; }
        public DbSet<Domain.Entities.SubTaskAnswer> SubTaskAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ProjectManagement");
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new SubtaskConfiguration());
            modelBuilder.ApplyConfiguration(new IndividualTaskConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}