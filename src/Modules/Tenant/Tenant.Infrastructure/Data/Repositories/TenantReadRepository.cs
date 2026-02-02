using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.Repositories
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
                "Tüm şirket planları getiriliyor - Değişiklik İzleme: {TrackChanges}",
                trackChanges);
            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;

                if (!trackChanges)
                    query = query.AsNoTracking();

                query = query
                    .Where(p=> p.isActive == true)
                    .OrderBy(p => p.PlanPrice)
                    .ThenBy(p => p.PlanName);

                _logger.LogInformation(
                    "{Count} adet şirket planı getirildi",
                    query.Count());
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Planlar getirilirken hata oluştu.");
                throw;
            }
        }
        public async Task<CompanyPlan> GetPlan(Guid id, bool trackChanges)
        {
            ValidateId(id);
            _logger.LogInformation(
                "Şirket planı getiriliyor - Id: {PlanId}, Değişiklik İzleme: {TrackChanges}",
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
                        "Şirket planı bulunamadı - Id: {PlanId}",
                        id);
                }
                else
                {
                    _logger.LogDebug(
                        "Şirket planı bulundu - Id: {PlanId}",
                        id);
                }
                return plan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Plan getirilirken hata oluştu - Id: {PlanId}",
                    id);
                throw;
            }
        }
        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Plan ID'si boş olamaz", nameof(id));
            }
        }



    }
}