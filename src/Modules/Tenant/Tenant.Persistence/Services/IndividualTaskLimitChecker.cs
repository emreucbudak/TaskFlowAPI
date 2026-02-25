using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Application.Repositories;

namespace Tenant.Persistence.Services
{
    public sealed class IndividualTaskLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly ITenantWriteRepository _tenantWriteRepository;

        public IndividualTaskLimitChecker(
            ITenantReadRepository tenantReadRepository,
            ITenantWriteRepository tenantWriteRepository)
        {
            _tenantReadRepository = tenantReadRepository;
            _tenantWriteRepository = tenantWriteRepository;
        }

        public LimitType LimitType => LimitType.IndividualTask;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var subscription = await _tenantReadRepository.GetTenantSubscriptionForLimits(companyId, CancellationToken.None);
            ArgumentNullException.ThrowIfNull(subscription, "Sirketinize ait bir abonelik bulunamadi!");

            var taskLimit = subscription.CompanyPlan.PlanProperties.IndividualTaskLimit;
            var isReserved = await _tenantWriteRepository.TryReserveLimitSlot(
                companyId,
                LimitType.IndividualTask,
                taskLimit,
                CancellationToken.None);

            if (!isReserved)
            {
                throw new SubscriptionLimitExceededException("Bireysel gorev icin belirlenen sinira ulastiniz.");
            }
        }

        public Task ReleaseLimitAsync(Guid companyId)
        {
            return _tenantWriteRepository.ReleaseReservedLimitSlot(companyId, LimitType.IndividualTask, CancellationToken.None);
        }
    }
}
