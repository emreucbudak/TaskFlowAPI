using Microsoft.EntityFrameworkCore;
using Report.Domain.Entities;
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

            modelBuilder.Entity<Domain.Entities.Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired();
                
                entity.HasOne(e => e.ReportTopic)
                    .WithMany()
                    .HasForeignKey(e => e.ReportTopicId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}