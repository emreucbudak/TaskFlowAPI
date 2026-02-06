using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Command.DeleteUserFromDepartment
{
    public record DeleteUserFromDepartmentCommandRequest : IRequest
    {
        public Guid DepartmentId { get; init; }
        public Guid UserId { get; init; }
    }
}