using FlashMediator;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.Subscription.Queries.GetCompanySubscriptionSnapshot;

public sealed class GetCompanySubscriptionSnapshotQueryHandler
    : IRequestHandler<GetCompanySubscriptionSnapshotQueryRequest, GetCompanySubscriptionSnapshotQueryResponse>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetCompanySubscriptionSnapshotQueryHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<GetCompanySubscriptionSnapshotQueryResponse> Handle(
        GetCompanySubscriptionSnapshotQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CompanyId == Guid.Empty)
        {
            throw new ArgumentException("CompanyId zorunludur.", nameof(request.CompanyId));
        }

        var subscription = await _tenantReadRepository.GetTenantSubscriptionForLimits(request.CompanyId, cancellationToken);
        if (subscription is null)
        {
            return new GetCompanySubscriptionSnapshotQueryResponse
            {
                CompanyId = request.CompanyId,
                HasActiveSubscription = false
            };
        }

        return new GetCompanySubscriptionSnapshotQueryResponse
        {
            CompanyId = request.CompanyId,
            HasActiveSubscription = true,
            PlanName = subscription.CompanyPlan?.PlanName ?? string.Empty,
            PlanPrice = subscription.CompanyPlan?.PlanPrice ?? 0,
            Status = subscription.Status.ToString(),
            StartDateUtc = subscription.StartDate,
            NextBillingDateUtc = subscription.NextBillingDate,
            CurrentUserCount = subscription.TenantUsage?.CurrentUserCount ?? 0,
            CurrentGroupCount = subscription.TenantUsage?.CurrentGroupCount ?? 0,
            CurrentIndividualTaskCount = subscription.TenantUsage?.CurrentIndividualTaskCount ?? 0,
            UserLimit = subscription.CompanyPlan?.PlanProperties?.PeopleAddedLimit ?? 0,
            TeamLimit = subscription.CompanyPlan?.PlanProperties?.TeamLimit ?? 0,
            IndividualTaskLimit = subscription.CompanyPlan?.PlanProperties?.IndividualTaskLimit ?? 0,
            IsInternalReportingEnabled = subscription.CompanyPlan?.PlanProperties?.IsInternalReportingEnabled ?? false
        };
    }
}
