using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Report.Application.Repositories;
using Report.Application.UnitOfWork;
using Report.Persistence.Data;
using Report.Persistence.Repositories;

namespace Report.Persistence.Extensions
{
    public static class ReportServiceExtensions
    {
        public static IServiceCollection AddReportModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReportDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ReportDbContext).Assembly.FullName));
            });

            services.AddScoped<IReportUnitOfWork, Data.UnitOfWork.UnitOfWork>();
            

            services.AddScoped<IReportReadRepository, ReportReadRepository>();
            services.AddScoped<IReportWriteRepository, ReportWriteRepository>();

            return services;
        }
    }
}
