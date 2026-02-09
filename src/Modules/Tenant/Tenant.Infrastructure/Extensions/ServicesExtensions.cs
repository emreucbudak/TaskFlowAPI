using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenant.Persistence.Extensions;

namespace Tenant.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddConfigureTenant(this IServiceCollection services, IConfiguration config)
        {
            services.AddTenantPersistence(config);
        }
    }
}