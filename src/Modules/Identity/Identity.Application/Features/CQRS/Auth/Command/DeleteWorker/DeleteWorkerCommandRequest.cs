using FlashMediator;

namespace Identity.Application.Features.CQRS.Auth.Command.DeleteWorker;

public sealed record DeleteWorkerCommandRequest : IRequest
{
    public Guid UserId { get; init; }
}
