using Microsoft.Extensions.Logging;
using Stats.Application.Repoitories;
using Stats.Domain.Entities;
using Stats.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Stats.Persistence.Repositories
{
    public class WorkerStatsWriteRepositories : IWorkerStatsWriteRepositories
    {
        private readonly StatsDbContext _context;
        private readonly ILogger<WorkerStatsWriteRepositories> _logger;

        public WorkerStatsWriteRepositories(StatsDbContext context, ILogger<WorkerStatsWriteRepositories> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Update(WorkerStats workerStats)
        {
            if (workerStats == null)
            {
                _logger.LogError("Güncelleme işlemi için null varlık gönderildi.");
                throw new ArgumentNullException(nameof(workerStats), "Güncellenecek veri null olamaz.");
            }

            try
            {
                _logger.LogInformation("WorkerStats güncelleniyor. ID: {Id}", workerStats.Id);
                _context.UserStats.Update(workerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WorkerStats güncellenirken hata oluştu.");
                throw new InvalidOperationException("Veri güncelleme işlemi başarısız oldu.", ex);
            }
        }

        public void Delete(WorkerStats workerStats)
        {
            if (workerStats == null)
            {
                _logger.LogError("Silme işlemi için null varlık gönderildi.");
                throw new ArgumentNullException(nameof(workerStats), "Silinecek veri null olamaz.");
            }

            try
            {
                _logger.LogInformation("WorkerStats siliniyor. ID: {Id}", workerStats.Id);
                _context.UserStats.Remove(workerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WorkerStats silinirken hata oluştu.");
                throw new InvalidOperationException("Veri silme işlemi başarısız oldu.", ex);
            }
        }

        public async Task<WorkerStats> GetOrCreateStatsAsync(Guid userId, DateOnly period)
        {
            var normalizedPeriod = new DateOnly(period.Year, period.Month, 1);
            
            try
            {
                var stats = await _context.UserStats
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.Period.Year == normalizedPeriod.Year && x.Period.Month == normalizedPeriod.Month);

                if (stats == null)
                {
                    _logger.LogInformation("Yeni ay için WorkerStats oluşturuluyor: {UserId}, {Month}/{Year}", userId, normalizedPeriod.Month, normalizedPeriod.Year);
                    stats = new WorkerStats(userId, normalizedPeriod);
                    await _context.UserStats.AddAsync(stats);
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WorkerStats getirilirken veya oluşturulurken hata oluştu. UserId: {UserId}, Period: {Month}/{Year}", userId, normalizedPeriod.Month, normalizedPeriod.Year);
                throw new InvalidOperationException("İstatistik kaydı hazırlanamadı.", ex);
            }
        }
    }
}
