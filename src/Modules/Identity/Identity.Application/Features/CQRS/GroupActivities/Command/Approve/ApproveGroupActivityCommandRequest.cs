using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupActivities.Command.Approve;

public sealed record ApproveGroupActivityCommandRequest : IRequest
{
    public Guid ActivityId { get; init; }
    public string? Note { get; init; }
}
