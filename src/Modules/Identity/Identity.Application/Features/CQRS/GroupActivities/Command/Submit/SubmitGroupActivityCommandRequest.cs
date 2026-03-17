using FlashMediator;

namespace Identity.Application.Features.CQRS.GroupActivities.Command.Submit;

public sealed record SubmitGroupActivityCommandRequest : IRequest
{
    public Guid GroupId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
