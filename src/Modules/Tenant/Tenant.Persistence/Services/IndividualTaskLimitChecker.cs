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
            ArgumentNullException.ThrowIfNull(individualTasks,"Şirket için tanımlı hak bulunamadı!");
            ArgumentNullException.ThrowIfNull(individualTasks.CompanyPlan,
    "Şirkete ait plan bulunamadı.");
            ArgumentNullException.ThrowIfNull(individualTasks.TenantUsage,
                "Kullanım kaydı bulunamadı.");
            int companyRight = individualTasks.CompanyPlan.PlanProperties.GetIndividualTaskLimit();
            int currentUsage = individualTasks.TenantUsage.GetCurrentIndividualTaskCount();

            if (currentUsage >= companyRight)
            {
                throw new SubscriptionLimitExceededException("Bireysel görev için belirlenen sınıra ulaştınız.");
            }
        }
    }
}
