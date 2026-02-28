namespace Tenant.Application.Features.CQRS.Subscription.Command.Activate
{
    public sealed record ActivateCompanySubscriptionCommandResponse
    {
        public Guid CompanyId { get; init; }
        public string PlanName { get; init; } = string.Empty;
        public string Status { get; init; } = "Aktif";
        public DateTime StartDateUtc { get; init; }
        public DateTime NextBillingDateUtc { get; init; }
    }
}
