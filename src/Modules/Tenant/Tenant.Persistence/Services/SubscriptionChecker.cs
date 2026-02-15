using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Services
{
    public class SubscriptionChecker : ISubscriptionChecker
    {
        private readonly TenantDbContext _context;

        public SubscriptionChecker(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckSubscriptionStatusAsync(Guid tenantId)
        {
            var subscription = await _context.tenantSubscriptions
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (subscription == null)
            {
                return false; 
            }

            return subscription.Status == SubscriptionStatus.Aktif;
        }
    }
}