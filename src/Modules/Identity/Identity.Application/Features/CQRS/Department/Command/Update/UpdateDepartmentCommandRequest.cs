using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.Update
{
    public record UpdateDepartmentCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public UpdateDepartmentCommandRequest(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
