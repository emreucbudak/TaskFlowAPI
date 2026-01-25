using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.Delete
{
    public record DeleteDepartmentCommandRequest : IRequest
    {
        public Guid DepartmentId { get; init; }
        public DeleteDepartmentCommandRequest(Guid departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
