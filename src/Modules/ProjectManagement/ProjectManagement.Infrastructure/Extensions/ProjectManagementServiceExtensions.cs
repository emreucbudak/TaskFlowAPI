using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;
using ProjectManagement.Infrastructure.Data.Repositories;
using ProjectManagement.Infrastructure.Data.UnitOfWork;
using TaskFlow.BuildingBlocks.UnitOfWork;

using ProjectManagement.Application.Messaging;
using ProjectManagement.Infrastructure.Messaging;

namespace ProjectManagement.Infrastructure.Extensions
{
    public static class ProjectManagementServiceExtensions
    {
        public static IServiceCollection AddProjectManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProjectManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ProjectManagementDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProjectManagementReadRepository, ProjectManagementReadRepository>();
            services.AddScoped<IProjectManagementWriteRepository, ProjectManagementWriteRepository>();

            services.AddScoped<IProjectManagementProducer, ProjectManagementProducer>();

            return services;
        }
    }
}