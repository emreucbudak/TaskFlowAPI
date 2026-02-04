using Microsoft.EntityFrameworkCore;
using Stats.Persistence.Data.DataConfiguration;

namespace Stats.Persistence.Data
{
    public class StatsDbContext : DbContext
    {
        public StatsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected StatsDbContext()
        {
        }
        
        public DbSet<Domain.Entities.WorkerStats> UserStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new WorkerStatsConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}