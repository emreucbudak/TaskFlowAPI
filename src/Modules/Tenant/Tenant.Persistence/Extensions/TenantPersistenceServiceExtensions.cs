using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenant.Application.Repositories;
using Tenant.Persistence.Data.Repositories;
using Tenant.Persistence.Data.TenantDb;
using Tenant.Persistence.Data.UnitOfWork;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Tenant.Persistence.Extensions
{
    public static class TenantPersistenceServiceExtensions
    {
        public static IServiceCollection AddTenantPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TenantDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName);
                });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITenantWriteRepository, TenantWriteRepository>();

            return services;
        }
    }
}