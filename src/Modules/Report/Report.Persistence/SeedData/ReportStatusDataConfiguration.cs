using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Domain.Entities;

namespace Report.Persistence.SeedData
{
    public class ReportStatusDataConfiguration : IEntityTypeConfiguration<Report.Domain.Entities.ReportStatus>
    {
        public void Configure(EntityTypeBuilder<ReportStatus> builder)
        {
            builder.HasData(
                new ReportStatus { Id = 1, Name = "Bildirildi" },
                new ReportStatus { Id = 2, Name = "İşleme Alındı" },
                new ReportStatus { Id = 3, Name = "Çözüldü" },
                new ReportStatus { Id = 4, Name = "Reddedildi" }
            );
        }
    }
}
