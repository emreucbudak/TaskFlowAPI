using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Report.Infrastructure.Extensions
{
    public static class ReportInfrastructureExtensions
    {
        public static IServiceCollection AddReportInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            return services;
        }
    }
}
