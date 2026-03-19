using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupEvents.Command.Create;

public sealed record CreateGroupEventCommandRequest : IRequest<Guid>
{
    public Guid GroupId { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartsAt { get; init; }
    public DateTime? EndsAt { get; init; }
    public string? MeetingLink { get; init; }
}
