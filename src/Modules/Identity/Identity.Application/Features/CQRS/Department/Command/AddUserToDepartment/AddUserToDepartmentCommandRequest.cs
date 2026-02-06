using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment
{
    public record AddUserToDepartmentCommandRequest : IRequest
    {
        public AddUserToDepartmentCommandRequest(Guid userId, Guid departmentId)
        {
            UserId = userId;
            DepartmentId = departmentId;
        }

        public Guid UserId { get; init; }
        public Guid DepartmentId { get; init; }
    }
}
