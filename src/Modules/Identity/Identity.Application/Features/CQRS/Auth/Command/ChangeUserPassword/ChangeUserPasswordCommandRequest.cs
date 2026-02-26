using FlashMediator;

namespace Identity.Application.Features.CQRS.Auth.Command.ChangeUserPassword;

public sealed record ChangeUserPasswordCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public string NewPassword { get; init; } = string.Empty;
}
