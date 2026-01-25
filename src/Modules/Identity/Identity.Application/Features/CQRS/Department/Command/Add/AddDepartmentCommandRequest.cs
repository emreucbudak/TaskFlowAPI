using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.Add
{
    public record AddDepartmentCommandRequest : IRequest
    {
        public string Name { get; init; }
        public Guid companyId { get; init; }
        public AddDepartmentCommandRequest(string name, Guid companyId)
        {
            Name = name;
            this.companyId = companyId;
        }
    }
}
