using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupActivities.Command.Reject;

public sealed record RejectGroupActivityCommandRequest : IRequest
{
    public Guid ActivityId { get; init; }
    public string? Note { get; init; }
}
