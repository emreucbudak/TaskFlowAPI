using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class GroupTaskLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;

        public GroupTaskLimitChecker(TenantDbContext context)
        {
            _context = context;
        }

        public LimitType LimitType => LimitType.GroupTask;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var individualTasks = await _context.tenantSubscriptions.Where(t => t.TenantId == companyId).Include(x => x.TenantUsage).Include(x => x.CompanyPlan).ThenInclude(x => x.PlanProperties).FirstOrDefaultAsync();
            ArgumentNullException.ThrowIfNull(individualTasks, "Şirketinize ait bir abonelik bulunamadı!");
            int currentGroupTaskCount = individualTasks.TenantUsage.CurrentTaskCount;
            int groupTaskLimit = individualTasks.CompanyPlan.PlanProperties.GroupTaskLimit;
            if (currentGroupTaskCount >= groupTaskLimit)
            {
                throw new SubscriptionLimitExceededException($"Grup görev limiti aşıldı! Mevcut grup görev sayısı: {currentGroupTaskCount}, Grup görev limiti: {groupTaskLimit}");
            }

        }
    }
}
