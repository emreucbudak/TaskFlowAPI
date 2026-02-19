using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Data.Repositories
{
    public sealed class TenantReadRepository : ITenantReadRepository
    {
        private readonly TenantDbContext _context;
        private readonly ILogger<TenantReadRepository> _logger;
        public TenantReadRepository(
            TenantDbContext context,
            ILogger<TenantReadRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CompanyPlan>> GetAllPlans(bool trackChanges)
        {
            _logger.LogInformation(
                "Tüm þirket planlarý getiriliyor - Deðiþiklik Ýzleme: {TrackChanges}",
                trackChanges);
            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans
                    .Include(p => p.PlanProperties);

                if (!trackChanges)
                    query = query.AsNoTracking();

                query = query
                    .Where(p=> p.isActive == true)
                    .OrderBy(p => p.PlanPrice)
                    .ThenBy(p => p.PlanName);

                _logger.LogInformation(
                    "{Count} adet þirket planý getirildi",
                    query.Count());
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Planlar getirilirken hata oluþtu.");
                throw;
            }
        }
        public async Task<CompanyPlan> GetPlan(Guid id, bool trackChanges)
        {
            ValidateId(id);
            _logger.LogInformation(
                "Þirket planý getiriliyor - Id: {PlanId}, Deðiþiklik Ýzleme: {TrackChanges}",
                id, trackChanges);
            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;

                if (!trackChanges)
                    query = query.AsNoTracking();

                var plan = await query.FirstOrDefaultAsync(p => p.Id == id);
                if (plan is null)
                {
                    _logger.LogWarning(
                        "Þirket planý bulunamadý - Id: {PlanId}",
                        id);
                }
                else
                {
                    _logger.LogDebug(
                        "Þirket planý bulundu - Id: {PlanId}",
                        id);
                }
                return plan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Plan getirilirken hata oluþtu - Id: {PlanId}",
                    id);
                throw;
            }
        }
        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Plan ID'si boþ olamaz", nameof(id));
            }
        }



    }
}
