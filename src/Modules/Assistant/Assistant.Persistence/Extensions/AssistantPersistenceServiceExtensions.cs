using Assistant.Application.Repositories;
using Assistant.Persistence.Data.AssistantDb;
using Assistant.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.EntityFrameworkCore;

namespace Assistant.Persistence.Extensions;

public static class AssistantPersistenceServiceExtensions
{
    public static IServiceCollection AddAssistantPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PgVectorConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("PgVector connection string tanimlanmamis.");
        }

        services.AddDbContext<AssistantDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(AssistantDbContext).Assembly.FullName);
                    npgsqlOptions.UseVector();
                });
        });

        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();

        return services;
    }
}
