using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFlow.BuildingBlocks.Extensions
{
    public static  class CAPExtensions
    {
        public static IServiceCollection RegisterCapExtensions(this IServiceCollection services,IConfiguration configuration,string groupName)
        {
            services.AddCap(options =>
            {
                options.UsePostgreSql(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    options.Schema = "cap";
                });
                options.UseDashboard();
                options.FailedRetryCount = 5;
                options.FailedRetryInterval = 60;
                options.SucceedMessageExpiredAfter = 24 * 3600;
                options.ConsumerThreadCount = 5;
                options.UseRabbitMQ(options =>
                {
                    options.HostName = configuration["RabbitMQ:HostName"];
                    options.Port = int.Parse(configuration["RabbitMQ:Port"]);
                    options.UserName = configuration["RabbitMQ:UserName"];
                    options.Password = configuration["RabbitMQ:Password"];
                    options.VirtualHost = configuration["RabbitMQ:VirtualHost"];
                    options.ExchangeName = configuration["RabbitMQ:ExchangeName"];
                });
                options.DefaultGroupName = groupName;
            });
            return services;
        }
    }
}
