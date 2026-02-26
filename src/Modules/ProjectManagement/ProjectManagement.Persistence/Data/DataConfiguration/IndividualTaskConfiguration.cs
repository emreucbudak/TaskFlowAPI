using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Persistence.Data.DataConfiguration
{
    public class IndividualTaskConfiguration : IEntityTypeConfiguration<IndividualTasks>
    {
        public void Configure(EntityTypeBuilder<IndividualTasks> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TaskTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(5000);

            builder.Property(t => t.AssignedUserId)
                .IsRequired();
            
            builder.Property(t => t.Deadline)
                .IsRequired();

            builder.HasIndex(t => t.TaskPriorityCategoryId);

            builder.HasOne(t => t.TaskPriority)
                .WithMany()
                .HasForeignKey(t => t.TaskPriorityCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
