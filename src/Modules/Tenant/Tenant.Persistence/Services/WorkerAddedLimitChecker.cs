using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class WorkerAddedLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;

        public WorkerAddedLimitChecker(TenantDbContext context)
        {
            _context = context;
        }

        public LimitType LimitType => LimitType.PeopleAdded;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var companySubscription = await  _context.tenantSubscriptions.Where(t => t.TenantId == companyId).Include(x => x.TenantUsage).Include(x => x.CompanyPlan).ThenInclude(x => x.PlanProperties).FirstOrDefaultAsync();
            ArgumentNullException.ThrowIfNull(companySubscription, "Şirketinize ait bir  abonelik bulunamadı!");
            int currentWorkerCount = companySubscription.TenantUsage.CurrentUserCount;
            int subscriptionRight = companySubscription.CompanyPlan.PlanProperties.PeopleAddedLimit;
            if (currentWorkerCount >= subscriptionRight)
            {
                throw new SubscriptionLimitExceededException("Çalışan ekleme konusunda sınıra ulaştınız!");


            }
        }
    }
}
