using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chat.Persistence.Data.ChatDb;
using Chat.Persistence.Data.UnitOfWork;
using Chat.Application.Repositories;
using Chat.Persistence.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;
using Chat.Application.Services;

namespace Chat.Persistence.Extensions
{
    public static class ChatServiceExtensions
    {
        public static IServiceCollection AddChatPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName));
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMessageReadRepository, MessageReadRepository>();
            services.AddScoped<IMessageWriteRepository, MessageWriteRepository>();
            services.AddScoped<IMessageControlService, MessageControlService>();

            return services;
        }
    }
}