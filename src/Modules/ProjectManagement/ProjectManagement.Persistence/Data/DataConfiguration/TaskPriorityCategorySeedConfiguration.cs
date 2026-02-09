using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Persistence.Data.DataConfiguration
{
    public class TaskPriorityCategorySeedConfiguration : IEntityTypeConfiguration<TaskPriorityCategory>
    {
        public void Configure(EntityTypeBuilder<TaskPriorityCategory> builder)
        {
            builder.HasData(new TaskPriorityCategory()
            {
                TaskPriorityCategoryId = 1,
                CategoryName = "Öncelikli Görev"
            },
            new TaskPriorityCategory()
            {
                TaskPriorityCategoryId = 2,
                CategoryName = "Sýradan Görev"
            },
            new TaskPriorityCategory()
            {
                TaskPriorityCategoryId = 3,
                CategoryName = "Acil Görev"
            });
        }
    }
}
