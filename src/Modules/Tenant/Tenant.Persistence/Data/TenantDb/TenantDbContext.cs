using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;
using Tenant.Domain.Entities;

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
        public DbSet<CompanyPlan> companyPlans { get; set; }
        public DbSet<PlanProperties> planProperties { get; set; }
        public DbSet<TenantSubscription> tenantSubscriptions { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Tenant");
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("AspNetUsers", "Identity");
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.ToTable("Groups", "Identity");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}