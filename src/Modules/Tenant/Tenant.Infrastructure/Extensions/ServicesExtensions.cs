using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tenant.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddConfigureTenant(this IServiceCollection services,IConfiguration config)
        {
            services.AddScoped<Application.UnitOfWork.IUnitOfWork, Data.UnitOfWork.UnitOfWork>();
            services.AddScoped<Application.Repositories.ITenantWriteRepository, Data.Repositories.TenantWriteRepository>();
            services.AddDbContext<Data.TenantDb.TenantDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("Tenant.Infrastructure");
                });
            });
            

        }
    }
}
