using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupEvents.Command.Delete;

public sealed record DeleteGroupEventCommandRequest : IRequest
{
    public Guid GroupEventId { get; init; }
}
