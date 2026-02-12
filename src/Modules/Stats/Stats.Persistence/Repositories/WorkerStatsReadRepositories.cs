using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stats.Application.Repositories;
using Stats.Domain.Entities;
using Stats.Persistence.Data;
using TaskFlow.BuildingBlocks.Common;

namespace Stats.Persistence.Repositories
{
    public class WorkerStatsReadRepositories : IWorkerStatsReadRepositories
    {
        private readonly StatsDbContext _context;
        private readonly ILogger<WorkerStatsReadRepositories> _logger;

        public WorkerStatsReadRepositories(StatsDbContext context, ILogger<WorkerStatsReadRepositories> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WorkerStats?> GetByUserAndPeriodAsync(Guid userId, DateOnly period, bool trackChanges)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("Geçersiz UserId parametresi: {UserId}", userId);
                throw new ArgumentException("UserId boş olamaz.", nameof(userId));
            }

            try
            {
                _logger.LogInformation("UserId ve periyot ile WorkerStats getiriliyor: {UserId}, {Month}/{Year}", userId, period.Month, period.Year);
                var query = trackChanges ? _context.UserStats : _context.UserStats.AsNoTracking();
                return await query.FirstOrDefaultAsync(x => x.UserId == userId && x.Period.Year == period.Year && x.Period.Month == period.Month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserId ve periyot ile WorkerStats getirilirken hata oluştu (UserId: {UserId}, {Month}/{Year})", userId, period.Month, period.Year);
                throw new InvalidOperationException("İstatistik getirilirken bir hata oluştu.", ex);
            }
        }

        public async Task<PagedResult<WorkerStats>> GetAllWorkersStatsByPeriodAsync(DateOnly period, int page, int pageSize, bool trackChanges)
        {
            try
            {
                _logger.LogInformation("Belirli bir periyot için tüm çalışan istatistikleri getiriliyor: {Month}/{Year}", period.Month, period.Year);
                
                var query = trackChanges ? _context.UserStats : _context.UserStats.AsNoTracking();
                var filteredQuery = query.Where(x => x.Period.Year == period.Year && x.Period.Month == period.Month);
                
                return await GetPagedResultAsync(filteredQuery, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Periyot bazlı çalışan istatistikleri getirilirken hata oluştu (Period: {Month}/{Year})", period.Month, period.Year);
                throw new InvalidOperationException("İstatistikler getirilirken bir hata oluştu.", ex);
            }
        }

        private async Task<PagedResult<WorkerStats>> GetPagedResultAsync(IQueryable<WorkerStats> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<WorkerStats>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}