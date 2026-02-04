using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stats.Application.Repoitories;
using Stats.Persistence.Data;
using Stats.Persistence.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Stats.Persistence.Extensions
{
    public static class StatsServiceExtensions
    {
        public static IServiceCollection AddStatsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StatsDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(StatsDbContext).Assembly.FullName));
            });

            services.AddScoped<IUnitOfWork, Data.UnitOfWork.UnitOfWork>();
            
            services.AddScoped<IWorkerStatsReadRepositories, WorkerStatsReadRepositories>();
            services.AddScoped<IWorkerStatsWriteRepositories, WorkerStatsWriteRepositories>();

            return services;
        }
    }
}
