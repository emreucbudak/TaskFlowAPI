using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupEvents.Queries.GetByGroup;

public sealed record GetGroupEventsQueryRequest : IRequest<List<GetGroupEventsQueryResponse>>
{
    public Guid GroupId { get; init; }
}
