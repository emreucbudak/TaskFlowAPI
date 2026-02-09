using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Persistence.Data.DataConfiguration
{
    public class TaskConfiguration : IEntityTypeConfiguration<ProjectManagement.Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<ProjectManagement.Domain.Entities.Task> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TaskName).IsRequired();
            builder.Property(x => x.Description).IsRequired();

            builder.HasIndex(x => x.TaskStatusId);
            builder.HasIndex(x => x.DeadlineTime);
            builder.HasIndex(x => x.TaskPriorityCategoryId);
            builder.HasIndex(x => x.CreatedDate);

            builder.HasOne(x => x.TaskStatus)
                .WithMany()
                .HasForeignKey(x => x.TaskStatusId);

            builder.HasOne(x => x.TaskPriority)
                .WithMany()
                .HasForeignKey(x => x.TaskPriorityCategoryId);
        }
    }
}
