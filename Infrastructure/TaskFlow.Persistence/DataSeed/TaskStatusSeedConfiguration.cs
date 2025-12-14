using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.DataSeed
{
    public class TaskStatusSeedConfiguration : IEntityTypeConfiguration<Domain.Entities.TaskStatus>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TaskStatus> builder)
        {
            builder.HasData(new Domain.Entities.TaskStatus()
            {
                TaskStatusId= 1,
                StatusName = "Görev Ataması Yapıldı",
            },
            new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 2,
                StatusName = "Tamamlandı"
            },
            new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 3,
                StatusName = "Onay Bekliyor"
            });
        }
    }
}
