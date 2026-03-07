using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Persistence.Data.ProjectManagementDb
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectManagementDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

