using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Report.Persistence.Data.DataConfiguration
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report.Domain.Entities.Report>
    {
        public void Configure(EntityTypeBuilder<Report.Domain.Entities.Report> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Description).IsRequired();
            
            builder.HasOne(e => e.ReportTopic)
                .WithMany()
                .HasForeignKey(e => e.ReportTopicId);

     
            builder.HasIndex(e => e.ReportingUserId);
            builder.HasIndex(e => e.ReportTopicId);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
