using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.ConfigurationData
{
    public class GroupActivityConfiguration : IEntityTypeConfiguration<GroupActivity>
    {
        public void Configure(EntityTypeBuilder<GroupActivity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .HasMaxLength(2000);

            builder.Property(a => a.ReviewNote)
                   .HasMaxLength(500);

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasOne(a => a.Group)
                   .WithMany()
                   .HasForeignKey(a => a.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.SubmittedByUser)
                   .WithMany()
                   .HasForeignKey(a => a.SubmittedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ReviewedByUser)
                   .WithMany()
                   .HasForeignKey(a => a.ReviewedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
