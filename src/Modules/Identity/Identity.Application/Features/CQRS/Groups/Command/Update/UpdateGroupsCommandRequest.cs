using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Command.Update
{
    public record UpdateGroupsCommandRequest : IRequest
    {
        public UpdateGroupsCommandRequest(Guid ıd, string name)
        {
            Id = ıd;
            Name = name;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
 
    }
}
