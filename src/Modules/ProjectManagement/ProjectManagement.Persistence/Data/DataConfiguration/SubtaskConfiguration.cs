using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Persistence.Data.DataConfiguration
{
    public class SubtaskConfiguration : IEntityTypeConfiguration<Subtask>
    {
        public void Configure(EntityTypeBuilder<Subtask> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TaskTitle).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).IsRequired();

            builder.HasIndex(x => x.TaskId);
            builder.HasIndex(x => x.AssignedUserId);
            builder.HasIndex(x => x.TaskStatusId);

            builder.HasOne(x => x.Task)
                .WithMany(x => x.Subtasks)
                .HasForeignKey(x => x.TaskId);

            builder.HasOne(x => x.TaskStatus)
                .WithMany()
                .HasForeignKey(x => x.TaskStatusId);
        }
    }
}
