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
                new ReportTopic { Id = 1, TopicName = "Hata Bildirimi" },
                new ReportTopic { Id = 2, TopicName = "Geri Bildirim" },
                new ReportTopic { Id = 3, TopicName = "Diğer" }
            );
        }
    }
}
