using Chat.Application.ChatNotification;
using Chat.Application.Services;
using Chat.Infrastructure.ChatNotification;
using Chat.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.Extensions
{
    public static class ChatInfrastructureServiceExtensions
    {
        public static IServiceCollection AddChatInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IChatNotificationService, ChatNotificationService>();
            services.AddScoped<IGroupValidationService, GroupValidationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}
