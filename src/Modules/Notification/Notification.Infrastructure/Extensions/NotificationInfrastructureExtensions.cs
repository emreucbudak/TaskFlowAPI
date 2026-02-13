using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Messaging.Consumers;

namespace Notification.Infrastructure.Extensions
{
    public static class NotificationInfrastructureExtensions
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<NotifyUserConsumer>();
            return services;
        }
    }
}
