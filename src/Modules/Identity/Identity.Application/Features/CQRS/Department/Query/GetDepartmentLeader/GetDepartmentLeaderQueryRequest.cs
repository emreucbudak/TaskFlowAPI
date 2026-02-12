using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public record GetDepartmentLeaderQueryRequest : IRequest<GetDepartmentLeaderQueryResponse>, ICacheableQuery
    {
        public GetDepartmentLeaderQueryRequest(Guid departmentId)
        {
            DepartmentId = departmentId;
        }

        public Guid DepartmentId { get; init; }
        public string CacheKey => "getdepartmentleader";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
