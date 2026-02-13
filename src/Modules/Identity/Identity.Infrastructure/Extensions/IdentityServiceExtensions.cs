using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.TokenService;
using Identity.Infrastructure.TokenService;
using Identity.Persistence.Extensions;
using Identity.Infrastructure.Messaging.Consumers;

namespace Identity.Infrastructure.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityPersistence(configuration);

            services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
            services.AddScoped<ITokenService, Infrastructure.TokenService.TokenService>();
            services.AddScoped<ReportCreatedConsumer>();

            return services;
        }
    }
}