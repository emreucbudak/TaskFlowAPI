using Microsoft.EntityFrameworkCore;

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
        }
}
