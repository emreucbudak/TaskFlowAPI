using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.DataSeed
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
                CategoryName = "Sıradan Görev"
            },
            new TaskPriorityCategory()
            {
                TaskPriorityCategoryId= 3,
                CategoryName = "Acil Görev"
            });
        }
    }
}
