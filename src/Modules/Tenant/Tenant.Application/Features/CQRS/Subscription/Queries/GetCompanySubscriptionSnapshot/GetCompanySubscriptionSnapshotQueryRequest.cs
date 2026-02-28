using FlashMediator;

namespace Tenant.Application.Features.CQRS.Subscription.Queries.GetCompanySubscriptionSnapshot;

public sealed class GetCompanySubscriptionSnapshotQueryRequest : IRequest<GetCompanySubscriptionSnapshotQueryResponse>
{
    public Guid CompanyId { get; init; }
}
