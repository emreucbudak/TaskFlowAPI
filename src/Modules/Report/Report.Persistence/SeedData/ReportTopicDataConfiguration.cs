using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Domain.Entities;
namespace Report.Persistence.SeedData
{
    public class ReportTopicDataConfiguration : IEntityTypeConfiguration<ReportTopic>
    {
        public void Configure(EntityTypeBuilder<ReportTopic> builder)
        {
            builder.HasData(new ReportTopic
            {
                Id = 1,
                Name = "Hata Bildirimi"
            },
            new ReportTopic
            {
                Id = 2,
                Name = "Diğer"
            },
            new ReportTopic
            {
                Id = 3,
                Name = "Geri Bildirim"
            });
        }
    }
}
