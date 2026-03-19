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
        public DbSet<CompanyPlan> CompanyPlans { get; set; }
        public DbSet<PlanProperties> PlanProperties { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<TenantUsage> TenantUsages { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Tenant");
            modelBuilder.Entity<TenantUsage>()
                .HasIndex(t => t.TenantId)
                .IsUnique();
            modelBuilder.Entity<TenantUsage>()
                .Property(t => t.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedNever();
            modelBuilder.Entity<TenantSubscription>()
                .Property(t => t.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedNever();
            modelBuilder.Entity<TenantSubscription>().HasOne(ts => ts.TenantUsage)
                .WithOne()
                .HasForeignKey<TenantSubscription>(ts => ts.TenantUsageId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CompanyPlan>(entity =>
            {
                entity.Property(cp => cp.PlanName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.HasOne(cp => cp.PlanProperties)
                    .WithOne()
                    .HasForeignKey<CompanyPlan>(cp => cp.PlanPropertiesId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasOne<TenantSubscription>()
                    .WithMany()
                    .HasForeignKey(pt => pt.TenantSubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(pt => pt.RowVersion)
                    .IsConcurrencyToken()
                    .ValueGeneratedNever();
            });
            modelBuilder.Entity<CompanyPlan>()
                .Property(cp => cp.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedNever();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
