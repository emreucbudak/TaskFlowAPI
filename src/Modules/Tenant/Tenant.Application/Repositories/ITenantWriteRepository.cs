using TaskFlow.BuildingBlocks.Enums;
using Tenant.Domain.Entities;

namespace Tenant.Application.Repositories
{
    public interface ITenantWriteRepository
    {
        Task AddPlan(CompanyPlan plan);
        Task DeletePlan (CompanyPlan plan);
        Task UpdatePlan (CompanyPlan plan);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<TenantUsage> GetOrCreateTenantUsage(Guid tenantId, CancellationToken cancellationToken);
        Task<bool> TryReserveLimitSlot(Guid tenantId, LimitType limitType, int limit, CancellationToken cancellationToken);
        Task ReleaseReservedLimitSlot(Guid tenantId, LimitType limitType, CancellationToken cancellationToken);
        Task<TenantSubscription?> GetTenantSubscription(Guid tenantId, CancellationToken cancellationToken);
        Task AddTenantSubscription(TenantSubscription tenantSubscription, CancellationToken cancellationToken);
        Task UpdateTenantSubscription(Guid tenantId, Guid companyPlanId, string paymentProviderSubscriptionId, DateTime utcNow, CancellationToken cancellationToken);
    }
}
