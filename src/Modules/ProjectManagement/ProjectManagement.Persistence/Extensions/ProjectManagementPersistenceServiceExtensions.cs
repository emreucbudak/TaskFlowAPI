using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Persistence.Data.ProjectManagementDb;
using ProjectManagement.Persistence.Data.Repositories;
using ProjectManagement.Persistence.Data.UnitOfWork;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Persistence.Extensions
{
    public static class ProjectManagementPersistenceServiceExtensions
    {
        public static IServiceCollection AddProjectManagementPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProjectManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ProjectManagementDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProjectManagementReadRepository, ProjectManagementReadRepository>();
            services.AddScoped<IProjectManagementWriteRepository, ProjectManagementWriteRepository>();

            return services;
        }
    }
}