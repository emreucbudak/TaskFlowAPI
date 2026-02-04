using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.DataConfiguration
{
    public class TaskAnswerConfiguration : IEntityTypeConfiguration<TaskAnswer>
    {
        public void Configure(EntityTypeBuilder<TaskAnswer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AnswerText).IsRequired();

 
            builder.HasIndex(x => x.SenderId);
            builder.HasIndex(x => x.TaskId);
            builder.HasIndex(x => x.CreatedDate);
        }
    }
}
