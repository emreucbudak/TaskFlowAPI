using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Messaging;
using ProjectManagement.Infrastructure.Messaging;
using ProjectManagement.Persistence.Extensions;

namespace ProjectManagement.Infrastructure.Extensions
{
    public static class ProjectManagementServiceExtensions
    {
        public static IServiceCollection AddProjectManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProjectManagementPersistence(configuration);

            services.AddScoped<IProjectManagementProducer, ProjectManagementProducer>();

            return services;
        }
    }
}