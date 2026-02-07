using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment
{
    public record AddUserToDepartmentCommandRequest : IRequest
    {
        public AddUserToDepartmentCommandRequest(Guid userId, Guid departmentId, int roleId)
        {
            UserId = userId;
            DepartmentId = departmentId;
            RoleId = roleId;
        }

        public Guid UserId { get; init; }
        public Guid DepartmentId { get; init; }
        public int RoleId { get; init; }
    }
}
