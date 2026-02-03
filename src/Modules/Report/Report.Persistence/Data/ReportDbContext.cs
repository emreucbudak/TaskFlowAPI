using Microsoft.EntityFrameworkCore;

namespace Report.Persistence.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Domain.Entities.Report> Reports { get; set; }
        public DbSet<Domain.Entities.ReportTopic> ReportTopics { get; set; }
    }
}
