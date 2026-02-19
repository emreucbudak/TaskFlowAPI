using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFlow.BuildingBlocks.Extensions
{
    public static  class CAPExtensions
    {
        public static IServiceCollection RegisterCapExtensions(this IServiceCollection services,IConfiguration configuration,string groupName = "taskflow.messaging")
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
                    var rabbitMqPortValue = configuration["RabbitMQ:Port"];
                    var rabbitMqPort = int.TryParse(rabbitMqPortValue, out var parsedPort) ? parsedPort : 5672;

                    options.HostName = configuration["RabbitMQ:HostName"] ?? "localhost";
                    options.Port = rabbitMqPort;
                    options.UserName = configuration["RabbitMQ:UserName"] ?? "guest";
                    options.Password = configuration["RabbitMQ:Password"] ?? "guest";
                    options.VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
                    options.ExchangeName = configuration["RabbitMQ:ExchangeName"] ?? "taskflow.exchange";
                });
                options.DefaultGroupName = groupName;
            });
     
            return services;
        }
    }
}
