using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pgvector.EntityFrameworkCore;

namespace Assistant.Persistence.Data.AssistantDb;

public sealed class AssistantDbContextFactory : IDesignTimeDbContextFactory<AssistantDbContext>
{
    public AssistantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AssistantDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=TaskFlowVectorDb;Username=postgres;Password=postgres",
            options =>
            {
                options.MigrationsAssembly(typeof(AssistantDbContext).Assembly.FullName);
                options.UseVector();
            });

        return new AssistantDbContext(optionsBuilder.Options);
    }
}
