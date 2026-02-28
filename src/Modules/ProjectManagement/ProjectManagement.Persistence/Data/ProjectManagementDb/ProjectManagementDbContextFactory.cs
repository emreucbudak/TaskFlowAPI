using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectManagement.Persistence.Data.ProjectManagementDb
{
    public sealed class ProjectManagementDbContextFactory : IDesignTimeDbContextFactory<ProjectManagementDbContext>
    {
        public ProjectManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectManagementDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=TaskFlowDb;Username=postgres;Password=postgres",
                options => options.MigrationsAssembly(typeof(ProjectManagementDbContext).Assembly.FullName));

            return new ProjectManagementDbContext(optionsBuilder.Options);
        }
    }
}
