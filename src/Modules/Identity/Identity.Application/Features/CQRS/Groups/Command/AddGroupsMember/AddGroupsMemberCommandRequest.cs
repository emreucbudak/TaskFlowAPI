using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.AddGroupsMember
{
    public record AddGroupsMemberCommandRequest : IRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
        public int RolesId { get; init; }
    }
}
