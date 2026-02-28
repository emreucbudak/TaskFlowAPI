using FlashMediator;
using FluentValidation;
using FluentValidation.Results;
using Tenant.Application.Features.CQRS.Subscription.Exceptions;
using Tenant.Application.Repositories;
using Tenant.Application.Services;
using Tenant.Domain.Entities;

namespace Tenant.Application.Features.CQRS.Subscription.Command.Activate
{
    public sealed class ActivateCompanySubscriptionCommandHandler : IRequestHandler<ActivateCompanySubscriptionCommandRequest, ActivateCompanySubscriptionCommandResponse>
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly ITenantWriteRepository _tenantWriteRepository;
        private readonly ICompanyDataResetService _companyDataResetService;

        public ActivateCompanySubscriptionCommandHandler(
            ITenantReadRepository tenantReadRepository,
            ITenantWriteRepository tenantWriteRepository,
            ICompanyDataResetService companyDataResetService)
        {
            _tenantReadRepository = tenantReadRepository;
            _tenantWriteRepository = tenantWriteRepository;
            _companyDataResetService = companyDataResetService;
        }

        public async Task<ActivateCompanySubscriptionCommandResponse> Handle(ActivateCompanySubscriptionCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty || (string.IsNullOrWhiteSpace(request.PlanSlug) && string.IsNullOrWhiteSpace(request.PlanName)))
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(request.CompanyId), "CompanyId ve plan bilgisi zorunludur.") });
            }

            var selectedPlan = await _tenantReadRepository.GetActivePlanByKey(request.PlanSlug, request.PlanName, cancellationToken);
            if (selectedPlan is null)
            {
                throw new CompanyPlanNotFoundExceptions();
            }

            var tenantUsage = await _tenantWriteRepository.GetOrCreateTenantUsage(request.CompanyId, cancellationToken);
            var existingSubscription = await _tenantWriteRepository.GetTenantSubscription(request.CompanyId, cancellationToken);

            var paymentProviderSubscriptionId = string.IsNullOrWhiteSpace(request.PaymentProviderSubscriptionId)
                ? $"manual-{Guid.NewGuid():N}"
                : request.PaymentProviderSubscriptionId.Trim();

            var utcNow = DateTime.UtcNow;
            var nextBillingDateUtc = utcNow.AddMonths(1);

            if (request.DeleteExistingCompanyData)
            {
                await _companyDataResetService.ResetCompanyDataAsync(request.CompanyId, cancellationToken);
            }

            if (request.ResetTenantUsage)
            {
                await _tenantWriteRepository.ResetTenantUsageCounters(request.CompanyId, cancellationToken);
            }

            if (existingSubscription is null)
            {
                var newSubscription = TenantSubscription.CreateActive(
                    request.CompanyId,
                    selectedPlan.Id,
                    tenantUsage.Id,
                    paymentProviderSubscriptionId,
                    utcNow);

                _tenantWriteRepository.AddTenantSubscription(newSubscription);
            }
            else
            {
                await _tenantWriteRepository.UpdateTenantSubscription(
                    request.CompanyId,
                    selectedPlan.Id,
                    paymentProviderSubscriptionId,
                    utcNow,
                    cancellationToken);
            }

            await _tenantWriteRepository.SaveChangesAsync(cancellationToken);

            return new ActivateCompanySubscriptionCommandResponse
            {
                CompanyId = request.CompanyId,
                PlanName = selectedPlan.PlanName,
                Status = "Aktif",
                StartDateUtc = utcNow,
                NextBillingDateUtc = nextBillingDateUtc
            };
        }
    }
}
