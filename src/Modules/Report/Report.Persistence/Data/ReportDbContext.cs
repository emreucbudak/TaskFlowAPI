using Microsoft.EntityFrameworkCore;
using Report.Domain.Entities;
using Report.Persistence.Data.DataConfiguration;
using Report.Persistence.SeedData;

namespace Report.Persistence.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Report> Reports { get; set; }
        public DbSet<ReportTopic> ReportTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ReportTopicDataConfiguration());
            modelBuilder.ApplyConfiguration(new ReportConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
