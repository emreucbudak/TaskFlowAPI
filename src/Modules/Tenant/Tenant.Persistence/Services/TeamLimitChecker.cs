using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Application.Repositories;

namespace Tenant.Persistence.Services
{
    public sealed class TeamLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly ITenantWriteRepository _tenantWriteRepository;

        public TeamLimitChecker(
            ITenantReadRepository tenantReadRepository,
            ITenantWriteRepository tenantWriteRepository)
        {
            _tenantReadRepository = tenantReadRepository;
            _tenantWriteRepository = tenantWriteRepository;
        }

        public LimitType LimitType => LimitType.TeamLimit;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var subscription = await _tenantReadRepository.GetTenantSubscriptionForLimits(companyId, CancellationToken.None);
            ArgumentNullException.ThrowIfNull(subscription, "Sirketinize ait bir abonelik bulunamadi!");

            var teamLimit = subscription.CompanyPlan.PlanProperties.TeamLimit;
            var isReserved = await _tenantWriteRepository.TryReserveLimitSlot(
                companyId,
                LimitType.TeamLimit,
                teamLimit,
                CancellationToken.None);

            if (!isReserved)
            {
                throw new SubscriptionLimitExceededException("Takim limiti asildi!");
            }
        }

        public Task ReleaseLimitAsync(Guid companyId)
        {
            return _tenantWriteRepository.ReleaseReservedLimitSlot(companyId, LimitType.TeamLimit, CancellationToken.None);
        }
    }
}
