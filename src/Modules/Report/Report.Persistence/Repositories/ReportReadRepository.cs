using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Report.Application.Repositories;
using Report.Persistence.Data;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Persistence.Repositories
{
    public class ReportReadRepository(
        ReportDbContext context,
        ILogger<ReportReadRepository> logger) : IReportReadRepository
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private DbSet<Domain.Entities.Report> db => context.Reports;

        public async Task<PagedResult<Domain.Entities.Report>> GetAllAsync(int pageSize,
            int page = 1,
            bool trackChanges = false,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null)
        {
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation("Raporlar listeleniyor - Sayfa: {Page}, Boyut: {PageSize}", page, pageSize);

            try
            {
                var query = trackChanges ? db.AsTracking() : db.AsNoTracking();

                if (inc != null)
                {
                    query = inc(query);
                }

                var totalCount = await query.CountAsync();
                var orderedQuery = query
                    .OrderByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
                
                var items = await orderedQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<Domain.Entities.Report>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Raporlar listelenirken bir hata oluştu.");
                throw new InvalidOperationException("Rapor verileri çekilirken bir hata oluştu. Lütfen parametreleri kontrol edin.", ex);
            }
        }

        public async Task<Domain.Entities.Report?> GetByIdAsync(
            bool trackChanges,
            Guid id,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning("Geçersiz ID ile sorgulama yapıldı (Empty Guid).");
                return null;
            }

            logger.LogInformation("Rapor getiriliyor - ID: {Id}", id);

            try
            {
                var query = trackChanges ? db.AsTracking() : db.AsNoTracking();

                if (inc != null)
                {
                    query = inc(query);
                }

                return await query.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rapor getirilirken hata oluştu. ID: {Id}", id);
                throw new InvalidOperationException($"{id} numaralı rapor getirilirken teknik bir hata oluştu.", ex);
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        }
    }
}
