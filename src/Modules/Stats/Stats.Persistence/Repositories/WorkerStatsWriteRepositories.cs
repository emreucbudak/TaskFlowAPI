using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stats.Application.Repositories;
using Stats.Domain.Entities;
using Stats.Persistence.Data;

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
                _logger.LogError("Guncelleme islemi icin null varlik gonderildi.");
                throw new ArgumentNullException(nameof(workerStats), "Guncellenecek veri null olamaz.");
            }

            try
            {
                _logger.LogInformation("WorkerStats guncelleniyor. ID: {Id}", workerStats.Id);
                _context.UserStats.Update(workerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WorkerStats guncellenirken hata olustu.");
                throw new InvalidOperationException("Veri guncelleme islemi basarisiz oldu.", ex);
            }
        }

        public void Delete(WorkerStats workerStats)
        {
            if (workerStats == null)
            {
                _logger.LogError("Silme islemi icin null varlik gonderildi.");
                throw new ArgumentNullException(nameof(workerStats), "Silinecek veri null olamaz.");
            }

            try
            {
                _logger.LogInformation("WorkerStats siliniyor. ID: {Id}", workerStats.Id);
                _context.UserStats.Remove(workerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WorkerStats silinirken hata olustu.");
                throw new InvalidOperationException("Veri silme islemi basarisiz oldu.", ex);
            }
        }

        public async Task<WorkerStats> GetOrCreateStatsAsync(Guid userId, DateOnly period)
        {
            var normalizedPeriod = new DateOnly(period.Year, period.Month, 1);

            try
            {
                var stats = await _context.UserStats
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.Period.Year == normalizedPeriod.Year &&
                        x.Period.Month == normalizedPeriod.Month);

                if (stats == null)
                {
                    _logger.LogInformation(
                        "Yeni ay icin WorkerStats olusturuluyor: {UserId}, {Month}/{Year}",
                        userId,
                        normalizedPeriod.Month,
                        normalizedPeriod.Year);

                    stats = new WorkerStats(userId, normalizedPeriod);
                    await _context.UserStats.AddAsync(stats);
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "WorkerStats getirilirken veya olusturulurken hata olustu. UserId: {UserId}, Period: {Month}/{Year}",
                    userId,
                    normalizedPeriod.Month,
                    normalizedPeriod.Year);

                throw new InvalidOperationException("Istatistik kaydi hazirlanamadi.", ex);
            }
        }

        public async Task RecordTaskCompletionAsync(
            Guid userId,
            DateOnly completedOn,
            DateOnly deadline,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId bos olamaz.", nameof(userId));
            }

            var period = new DateOnly(completedOn.Year, completedOn.Month, 1);

            try
            {
                var stats = await GetOrCreateStatsAsync(userId, period);
                stats.RecordTaskCompleted(completedOn, deadline);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Task completion puani kaydedilemedi. UserId: {UserId}, CompletedOn: {CompletedOn}, Deadline: {Deadline}",
                    userId,
                    completedOn,
                    deadline);

                throw;
            }
        }
    }
}
