using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.Delete
{
    public record DeleteGroupsCommandRequest : IRequest
    {
        public Guid GroupId { get; init; }
        public DeleteGroupsCommandRequest(Guid groupId)
        {
            GroupId = groupId;
        }
    }
}
