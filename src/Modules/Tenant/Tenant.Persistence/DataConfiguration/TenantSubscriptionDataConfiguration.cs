using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenant.Domain.Entities;

namespace Tenant.Persistence.DataConfiguration
{
    public class TenantSubscriptionDataConfiguration : IEntityTypeConfiguration<TenantSubscription>
    {
        public void Configure(EntityTypeBuilder<TenantSubscription> builder)
        {
            builder.Property(x => x.Status)
                       .HasConversion<string>() 
                       .HasMaxLength(20)      
                       .IsRequired();
        }
    }
}
