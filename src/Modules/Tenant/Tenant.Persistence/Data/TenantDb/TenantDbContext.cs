using Microsoft.EntityFrameworkCore;

namespace Tenant.Persistence.Data.TenantDb
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }

        protected TenantDbContext()
        {
        }
        public DbSet<Tenant.Domain.Entities.CompanyPlan> companyPlans { get; set; }
        public DbSet<Tenant.Domain.Entities.PlanProperties> planProperties { get; set; }
        public DbSet<Tenant.Domain.Entities.TenantSubscription> tenantSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Tenant");
            base.OnModelCreating(modelBuilder);
        }
    }
}
