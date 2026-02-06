using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.DeleteGroupsMember
{
    public record DeleteGroupsMemberCommandRequest : IRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
    }
}