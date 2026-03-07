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
                var query = BuildQuery(trackChanges, inc);
                return await ToPagedResultAsync(query, page, pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Raporlar listelenirken bir hata olustu.");
                throw new InvalidOperationException("Rapor verileri cekilirken bir hata olustu. Lutfen parametreleri kontrol edin.", ex);
            }
        }

        public async Task<PagedResult<Domain.Entities.Report>> GetByDepartmentAsync(Guid departmentId,
            int pageSize,
            int page = 1,
            bool trackChanges = false,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null)
        {
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation("Departman raporlari listeleniyor - Departman: {DepartmentId}, Sayfa: {Page}, Boyut: {PageSize}", departmentId, page, pageSize);

            try
            {
                var query = BuildQuery(trackChanges, inc)
                    .Where(report => report.NotifiedDepartmantId == departmentId);

                return await ToPagedResultAsync(query, page, pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Departman raporlari listelenirken bir hata olustu. Departman: {DepartmentId}", departmentId);
                throw new InvalidOperationException("Departman raporlari cekilirken bir hata olustu.", ex);
            }
        }

        public async Task<Domain.Entities.Report?> GetByIdAsync(
            bool trackChanges,
            Guid id,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning("Gecersiz ID ile sorgulama yapildi (Empty Guid).");
                return null;
            }

            logger.LogInformation("Rapor getiriliyor - ID: {Id}", id);

            try
            {
                var query = BuildQuery(trackChanges, inc);
                return await query.FirstOrDefaultAsync(item => item.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rapor getirilirken hata olustu. ID: {Id}", id);
                throw new InvalidOperationException($"{id} numarali rapor getirilirken teknik bir hata olustu.", ex);
            }
        }

        private IQueryable<Domain.Entities.Report> BuildQuery(
            bool trackChanges,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc)
        {
            var query = trackChanges ? db.AsTracking() : db.AsNoTracking();

            if (inc != null)
            {
                query = inc(query);
            }

            return query;
        }

        private static async Task<PagedResult<Domain.Entities.Report>> ToPagedResultAsync(IQueryable<Domain.Entities.Report> query, int page, int pageSize)
        {
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

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        }
    }
}