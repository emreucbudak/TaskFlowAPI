using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;
using TaskFlow.BuildingBlocks.Enums;
using Tenant.Domain.Entities;

namespace Tenant.Persistence.Services
{
    public class SubscriptionLimitCheckerService : ISubscriptionLimitCheckerService
    {
        private readonly TenantDbContext _context;

        public SubscriptionLimitCheckerService(TenantDbContext context)
        {
            _context = context;
        }

        public async Task CheckUserLimitAsync(Guid tenantId)
        {
            var subscription = await GetActiveSubscriptionAsync(tenantId);
            if (subscription == null) return; 

            var limit = subscription.CompanyPlan.PlanProperties.PeopleAddedLimit;
            if (limit == -1) return;

            var currentCount = await GetUserCountAsync(tenantId);
            
            if (currentCount >= limit)
            {
                throw new SubscriptionLimitExceededException($"User limit of {limit} reached for tenant {tenantId}.");
            }
        }

        public async Task CheckProjectLimitAsync(Guid tenantId)
        {
            var subscription = await GetActiveSubscriptionAsync(tenantId);
            if (subscription == null) return;

            var limit = subscription.CompanyPlan.PlanProperties.TeamLimit; 
            if (limit == -1) return;

            var currentCount = await GetProjectCountAsync(tenantId);

            if (currentCount >= limit)
            {
                throw new SubscriptionLimitExceededException($"Project/Team limit of {limit} reached for tenant {tenantId}.");
            }
        }

        public async Task CheckStorageLimitAsync(Guid tenantId, long fileSizeInBytes)
        {
             await Task.CompletedTask;
        }

        private async Task<TenantSubscription?> GetActiveSubscriptionAsync(Guid tenantId)
        {
            return await _context.tenantSubscriptions
                .Include(s => s.CompanyPlan)
                .ThenInclude(p => p.PlanProperties)
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Aktif);
        }

        private async Task<int> GetUserCountAsync(Guid tenantId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == tenantId)
                .CountAsync();
        }

        private async Task<int> GetProjectCountAsync(Guid tenantId)
        {
            return await _context.Groups
                .Where(g => g.CompanyId == tenantId)
                .CountAsync();
        }
    }
}