using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.Repositories;
using Identity.Persistence.Data.IdentityDb;
using Identity.Persistence.Data.UnitOfWork;
using Identity.Persistence.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Persistence.Extensions
{
    public static class IdentityPersistenceServiceExtensions
    {
        public static IServiceCollection AddIdentityPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityManagementDbContext).Assembly.FullName)));

            services.AddScoped<ICapUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ICapUnitOfWork>());
            
            services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            return services;
        }
    }
}
