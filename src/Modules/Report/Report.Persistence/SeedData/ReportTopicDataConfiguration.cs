using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Domain.Entities;

namespace Report.Persistence.SeedData
{
    public class ReportTopicDataConfiguration : IEntityTypeConfiguration<ReportTopic>
    {
        public void Configure(EntityTypeBuilder<ReportTopic> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasData(
                ReportTopic.CreateSeed(1, "Hata Bildirimi"),
                ReportTopic.CreateSeed(2, "Geri Bildirim"),
                ReportTopic.CreateSeed(3, "Diger")
            );
        }
    }
}
