namespace Tenant.Application.Features.CQRS.Subscription.Queries.GetCompanySubscriptionSnapshot;

public sealed record GetCompanySubscriptionSnapshotQueryResponse
{
    public Guid CompanyId { get; init; }
    public bool HasActiveSubscription { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int PlanPrice { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? StartDateUtc { get; init; }
    public DateTime? NextBillingDateUtc { get; init; }
    public int CurrentUserCount { get; init; }
    public int CurrentGroupCount { get; init; }
    public int CurrentIndividualTaskCount { get; init; }
    public int UserLimit { get; init; }
    public int TeamLimit { get; init; }
    public int IndividualTaskLimit { get; init; }
    public bool IsInternalReportingEnabled { get; init; }
}
