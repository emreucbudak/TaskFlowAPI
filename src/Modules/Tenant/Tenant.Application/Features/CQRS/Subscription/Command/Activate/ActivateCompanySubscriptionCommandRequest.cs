using FlashMediator;

namespace Tenant.Application.Features.CQRS.Subscription.Command.Activate
{
    public sealed class ActivateCompanySubscriptionCommandRequest : IRequest<ActivateCompanySubscriptionCommandResponse>
    {
        public Guid CompanyId { get; init; }
        public string? PlanSlug { get; init; }
        public string? PlanName { get; init; }
        public string? PaymentProviderSubscriptionId { get; init; }
        public bool DeleteExistingCompanyData { get; init; }
        public bool ResetTenantUsage { get; init; }
    }
}
