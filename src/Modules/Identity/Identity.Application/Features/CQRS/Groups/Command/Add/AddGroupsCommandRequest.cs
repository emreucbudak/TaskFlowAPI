using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.Add
{
    public class AddGroupsCommandRequest : IRequest
    {
        public string Name { get; set; }

        public AddGroupsCommandRequest(string name)
        {
            Name = name;
        }
    }
}
