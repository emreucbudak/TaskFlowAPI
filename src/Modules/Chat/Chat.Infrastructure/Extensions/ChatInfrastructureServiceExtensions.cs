using Chat.Application.ChatNotification;
using Chat.Infrastructure.ChatNotification;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.Extensions
{
    public static class ChatInfrastructureServiceExtensions
    {
        public static IServiceCollection AddChatInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IChatNotificationService, ChatNotificationService>();
            return services;
        }
    }
}
