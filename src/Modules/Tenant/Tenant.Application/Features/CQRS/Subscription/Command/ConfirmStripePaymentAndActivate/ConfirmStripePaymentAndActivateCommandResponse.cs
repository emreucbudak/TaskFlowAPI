namespace Tenant.Application.Features.CQRS.Subscription.Command.ConfirmStripePaymentAndActivate;

public sealed record ConfirmStripePaymentAndActivateCommandResponse
{
    public Guid CompanyId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public string Status { get; init; } = "Aktif";
    public DateTime StartDateUtc { get; init; }
    public DateTime NextBillingDateUtc { get; init; }
    public bool Processed { get; init; }
    public bool AlreadyProcessed { get; init; }
    public string Message { get; init; } = string.Empty;
}
