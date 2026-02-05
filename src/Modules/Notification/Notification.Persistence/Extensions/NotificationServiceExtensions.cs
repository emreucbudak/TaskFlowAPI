using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Repositories;
using Notification.Infrastructure.Data.NotificationDb;
using Notification.Persistence.Data.UnitOfWork;
using Notification.Persistence.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Persistence.Extensions
{
    public static class NotificationServiceExtensions
    {
        public static IServiceCollection AddNotificationPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<INotificationWriteRepository, NotificationWriteRepository>();

            return services;
        }
    }
}