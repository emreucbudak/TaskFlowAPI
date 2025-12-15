using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManagement.Infrastructure.Data.DataConfiguration
{
    public class TaskStatusSeedConfiguration : IEntityTypeConfiguration<Domain.Entities.TaskStatus>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TaskStatus> builder)
        {
            builder.HasData(new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 1,
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
