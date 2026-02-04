using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stats.Domain.Entities;

namespace Stats.Persistence.Data.DataConfiguration
{
    public class WorkerStatsConfiguration : IEntityTypeConfiguration<WorkerStats>
    {
        public void Configure(EntityTypeBuilder<WorkerStats> builder)
        {
            builder.HasKey(x => x.Id);

   
            builder.HasIndex(x => new { x.UserId, x.Period }).IsUnique();

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Period).IsRequired();
        }
    }
}
