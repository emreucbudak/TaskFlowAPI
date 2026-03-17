using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupActivities.Queries.GetByGroup;

public sealed record GetGroupActivitiesQueryRequest : IRequest<List<GetGroupActivitiesQueryResponse>>
{
    public Guid GroupId { get; init; }
}
