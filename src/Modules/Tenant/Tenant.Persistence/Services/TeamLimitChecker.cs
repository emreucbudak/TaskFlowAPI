using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class TeamLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;

        public TeamLimitChecker(TenantDbContext context)
        {
            _context = context;
        }

        public LimitType LimitType => LimitType.TeamLimit;

        public async Task CheckLimitAsync(Guid companyId)
        {
           var tenantSubscription = _context.tenantSubscriptions.Where(t => t.TenantId == companyId).Include(x => x.TenantUsage).Include(x => x.CompanyPlan).ThenInclude(x => x.PlanProperties).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(tenantSubscription, "Şirketinize ait bir abonelik bulunamadı!");
            int currentTeamCount = tenantSubscription.TenantUsage.CurrentGroupCount;
            int teamLimit = tenantSubscription.CompanyPlan.PlanProperties.TeamLimit;
            if (currentTeamCount >= teamLimit)
            {
                throw new SubscriptionLimitExceededException($"Takım limiti aşıldı! Mevcut takım sayısı: {currentTeamCount}, Takım limiti: {teamLimit}");
            }

        }
    }
}
