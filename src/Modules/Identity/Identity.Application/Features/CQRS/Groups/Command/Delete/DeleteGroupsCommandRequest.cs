using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.Delete
{
    public class DeleteGroupsCommandRequest : IRequest
    {
        public int GroupId { get; set; }
        public DeleteGroupsCommandRequest(int groupId)
        {
            GroupId = groupId;
        }
    }
}
