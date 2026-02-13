using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Report.Application.Messaging;
using Report.Infrastructure.Messaging;

namespace Report.Infrastructure.Extensions
{
    public static class ReportInfrastructureExtensions
    {
        public static IServiceCollection AddReportInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IReportProducer, ReportProducer>();
            services.AddCap(options =>
            {
                options.UsePostgreSql(pgOptions =>
                {
                    pgOptions.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    pgOptions.Schema = "cap";
                });
            });

            return services;
        }
    }
}
