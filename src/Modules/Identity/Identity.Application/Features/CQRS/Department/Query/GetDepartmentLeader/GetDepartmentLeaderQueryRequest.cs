using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public record GetDepartmentLeaderQueryRequest(Guid DepartmentId) : IRequest<Guid?>;
}
