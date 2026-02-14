using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenant.Persistence.Data.SeedData
{
    public class SubscriptionStatus : IEntityTypeConfiguration<Domain.Entities.SubscriptionStatus>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.SubscriptionStatus> builder)
        {
            builder.HasData(
                new Domain.Entities.SubscriptionStatus { Id = 1, Name = "Aktif" },
                new Domain.Entities.SubscriptionStatus { Id = 2, Name = "Inaktif" },
                new Domain.Entities.SubscriptionStatus { Id = 3, Name = "İptal Edildi" },
                new Domain.Entities.SubscriptionStatus { Id = 4, Name = "Ödeme Alınamadı" }

            );
        }
    }
}
