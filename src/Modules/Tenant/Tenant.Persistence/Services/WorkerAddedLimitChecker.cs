using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;
using Tenant.Application.Repositories;

namespace Tenant.Persistence.Services
{
    public sealed class WorkerAddedLimitChecker : ISubscriptionLimitCheckerService
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly ITenantWriteRepository _tenantWriteRepository;

        public WorkerAddedLimitChecker(
            ITenantReadRepository tenantReadRepository,
            ITenantWriteRepository tenantWriteRepository)
        {
            _tenantReadRepository = tenantReadRepository;
            _tenantWriteRepository = tenantWriteRepository;
        }

        public LimitType LimitType => LimitType.PeopleAdded;

        public async Task CheckLimitAsync(Guid companyId)
        {
            var subscription = await _tenantReadRepository.GetTenantSubscriptionForLimits(companyId, CancellationToken.None);
            ArgumentNullException.ThrowIfNull(subscription, "Sirketinize ait bir abonelik bulunamadi!");

            var workerLimit = subscription.CompanyPlan.PlanProperties.PeopleAddedLimit;
            var isReserved = await _tenantWriteRepository.TryReserveLimitSlot(
                companyId,
                LimitType.PeopleAdded,
                workerLimit,
                CancellationToken.None);

            if (!isReserved)
            {
                throw new SubscriptionLimitExceededException("Calisan ekleme konusunda sinira ulastiniz!");
            }
        }

        public Task ReleaseLimitAsync(Guid companyId)
        {
            return _tenantWriteRepository.ReleaseReservedLimitSlot(companyId, LimitType.PeopleAdded, CancellationToken.None);
        }
    }
}
