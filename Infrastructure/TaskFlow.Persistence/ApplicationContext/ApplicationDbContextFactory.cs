using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskFlow.Persistence.ApplicationContext
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = "Host=localhost;Port=5432;Database=TaskFlowDb;Username=postgres;Password=emreraftongame63";
            optionsBuilder.UseNpgsql(
                connectionString
            );
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
