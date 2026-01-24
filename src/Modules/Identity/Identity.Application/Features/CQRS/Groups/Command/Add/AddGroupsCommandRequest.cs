using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.Add
{
    public class AddGroupsCommandRequest : IRequest
    {
        public string Name { get; set; }
        public Guid companyId { get; set; }

        public AddGroupsCommandRequest(string name, Guid companyId)
        {
            Name = name;
            this.companyId = companyId;
        }
    }
}
