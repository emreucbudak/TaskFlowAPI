using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.ConfigurationData
{
    public class GroupEventConfiguration : IEntityTypeConfiguration<GroupEvent>
    {
        public void Configure(EntityTypeBuilder<GroupEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Subject)
                   .IsRequired()
                   .HasMaxLength(120);

            builder.Property(e => e.EventType)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .HasMaxLength(2000);

            builder.Property(e => e.StartsAt)
                   .IsRequired();

            builder.Property(e => e.EndsAt);

            builder.Property(e => e.MeetingLink)
                   .HasMaxLength(2000);

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.HasIndex(e => e.CreatedByUserId);
            builder.HasIndex(e => new { e.GroupId, e.StartsAt });

            builder.HasOne(e => e.Group)
                   .WithMany()
                   .HasForeignKey(e => e.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(e => e.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
