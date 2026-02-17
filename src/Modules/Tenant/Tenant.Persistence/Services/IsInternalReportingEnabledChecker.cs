using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class IsInternalReportingEnabledChecker : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;
        public IsInternalReportingEnabledChecker(TenantDbContext context)
        {
            _context = context;
        }
        public LimitType LimitType => LimitType.IsIncludeReporting;
        public async Task CheckLimitAsync(Guid companyId)
        {
            var tenantSubscription = await _context.tenantSubscriptions.Where(t => t.TenantId == companyId).Include(x => x.CompanyPlan).ThenInclude(x => x.PlanProperties).FirstOrDefaultAsync();
            ArgumentNullException.ThrowIfNull(tenantSubscription, "Şirketinize ait bir abonelik bulunamadı!");
            bool isInternalReportingEnabled = tenantSubscription.CompanyPlan.PlanProperties.IsInternalReportingEnabled;
            if (!isInternalReportingEnabled)
            {
                throw new SubscriptionLimitExceededException("Aboneliğiniz iç raporlama özelliğini içermiyor.");
            }
        }
    }
}
