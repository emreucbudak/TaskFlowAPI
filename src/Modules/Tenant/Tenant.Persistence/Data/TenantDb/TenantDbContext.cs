using Microsoft.EntityFrameworkCore;
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
        public DbSet<TenantUsage> tenantUsages { get; set; }
        public DbSet<PaymentTransaction> paymentTransactions { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Tenant");
            modelBuilder.Entity<TenantUsage>()
                .HasIndex(t => t.TenantId)
                .IsUnique();
            modelBuilder.Entity<TenantUsage>()
                .Property(t => t.RowVersion)
                .IsConcurrencyToken();
            modelBuilder.Entity<TenantSubscription>().HasOne(ts => ts.TenantUsage)
                .WithOne()
                .HasForeignKey<TenantSubscription>(ts => ts.TenantUsageId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
