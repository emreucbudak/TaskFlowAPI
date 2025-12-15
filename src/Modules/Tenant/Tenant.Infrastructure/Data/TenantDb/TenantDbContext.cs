using Microsoft.EntityFrameworkCore;

namespace Tenant.Infrastructure.Data.TenantDb
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions options) : base(options)
        {
        }

        protected TenantDbContext()
        {
        }
        public DbSet<Tenant.Domain.Entities.CompanyPlan> companyPlans { get; set; }
        public DbSet<Tenant.Domain.Entities.PlanProperties> planProperties { get; set; }
    }
}
