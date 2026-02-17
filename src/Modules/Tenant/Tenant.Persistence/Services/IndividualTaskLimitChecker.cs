using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class IndividualTaskLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;

        public IndividualTaskLimitChecker(TenantDbContext context)
        {
            _context = context;
        }

        public LimitType LimitType => LimitType.IndividualTask;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var individualTasks = await _context.tenantSubscriptions.Where(t => t.TenantId == companyId).Include(x=> x.TenantUsage).Include(x=> x.CompanyPlan).ThenInclude(x=> x.PlanProperties).FirstOrDefaultAsync();
            ArgumentNullException.ThrowIfNull(individualTasks,"Şirketinize ait bir abonelik bulunamadı!");
            int companyRight = individualTasks.CompanyPlan.PlanProperties.IndividualTaskLimit;
            int currentUsage = individualTasks.TenantUsage.CurrentIndividualTaskCount;

            if (currentUsage >= companyRight)
            {
                throw new SubscriptionLimitExceededException("Bireysel görev için belirlenen sınıra ulaştınız.");
            }
        }
    }
}
