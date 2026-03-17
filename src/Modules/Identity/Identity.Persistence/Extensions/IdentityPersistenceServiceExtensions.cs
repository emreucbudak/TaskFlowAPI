using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Identity.Persistence.Data.IdentityDb;
using Identity.Persistence.Data.UnitOfWork;
using Identity.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Persistence.Extensions
{
    public static class IdentityPersistenceServiceExtensions
    {
        public static IServiceCollection AddIdentityPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityManagementDbContext).Assembly.FullName)));

            services.AddScoped<IIdentityCapUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddScoped<IDepartmentReadRepository, ReadRepository<DepartmentMember, int>>();
            services.AddScoped<IDepartmentMemberRepository, DepartmentMemberRepository>();

            return services;
        }
    }
}

