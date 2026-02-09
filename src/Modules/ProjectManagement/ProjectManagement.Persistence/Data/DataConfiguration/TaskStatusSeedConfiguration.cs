using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManagement.Persistence.Data.DataConfiguration
{
    public class TaskStatusSeedConfiguration : IEntityTypeConfiguration<Domain.Entities.TaskStatus>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.TaskStatus> builder)
        {
            builder.HasData(new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 1,
                StatusName = "Görev Atamasý Yapýldý",
            },
            new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 2,
                StatusName = "Yapým Aþamasýnda"
            },
            new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 3,
                StatusName = "Onay Bekliyor"
            }, new Domain.Entities.TaskStatus()
            {
                TaskStatusId = 4,
                StatusName = "Tamamlandý"
            });
        }
    }
}
