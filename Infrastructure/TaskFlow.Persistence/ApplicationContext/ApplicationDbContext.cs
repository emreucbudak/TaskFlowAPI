using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskFlow.Domain.Bases;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.ApplicationContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Roles, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }
        public DbSet<Company> companies { get; set; }
        public DbSet<CompanyPlan> companiesPlan { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<PlanProperties> planProperties { get; set; }
        public DbSet<Domain.Entities.Task> tasks { get; set; }
        public DbSet<TaskAnswer> taskAnswers { get; set; }
        public DbSet<TaskPriorityCategory> taskPriorityCategories { get; set; }
        public DbSet<Domain.Entities.TaskStatus> taskStatuses { get; set; }
        public DbSet<SubTask> subTasks { get; set; }
        public DbSet<SubTaskAnswer> subTasksAnswer { get; set; }
        public DbSet<SubTaskStatus> subTasksStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
