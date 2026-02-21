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
            _logger.LogInformation("Loading all company plans. TrackChanges={TrackChanges}", trackChanges);
            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans.Include(p => p.PlanProperties);

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                query = query
                    .Where(p => p.isActive)
                    .OrderBy(p => p.PlanPrice)
                    .ThenBy(p => p.PlanName);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load company plans.");
                throw;
            }
        }

        public async Task<CompanyPlan?> GetActivePlanByKey(string? planSlug, string? planName, CancellationToken cancellationToken)
        {
            var plans = await _context.companyPlans
                .AsNoTracking()
                .Where(x => x.isActive)
                .ToListAsync(cancellationToken);

            return plans.FirstOrDefault(plan => MatchesPlan(plan.PlanName, planSlug, planName));
        }

        public async Task<CompanyPlan> GetPlan(Guid id, bool trackChanges)
        {
            ValidateId(id);
            _logger.LogInformation("Loading company plan. PlanId={PlanId}, TrackChanges={TrackChanges}", id, trackChanges);

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var plan = await query.FirstOrDefaultAsync(p => p.Id == id);
                return plan!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load company plan. PlanId={PlanId}", id);
                throw;
            }
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Plan id cannot be empty.", nameof(id));
            }
        }

        private static bool MatchesPlan(string planName, string? planSlug, string? rawPlanName)
        {
            var normalizedPlanName = NormalizePlanKey(planName);

            var slugMatch = !string.IsNullOrWhiteSpace(planSlug)
                && normalizedPlanName == NormalizePlanKey(planSlug);

            var nameMatch = !string.IsNullOrWhiteSpace(rawPlanName)
                && normalizedPlanName == NormalizePlanKey(rawPlanName);

            return slugMatch || nameMatch;
        }

        private static string NormalizePlanKey(string value)
        {
            var chars = value
                .Trim()
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray();

            return new string(chars);
        }
    }
}
