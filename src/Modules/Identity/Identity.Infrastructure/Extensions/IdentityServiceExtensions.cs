using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.Repositories;
using Identity.Application.TokenService;
using Identity.Infrastructure.Data.IdentityDb;
using Identity.Infrastructure.Data.UnitOfWork;
using Identity.Infrastructure.Infrastructure.TokenService;
using Identity.Infrastructure.Repository;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Infrastructure.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityManagementDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}