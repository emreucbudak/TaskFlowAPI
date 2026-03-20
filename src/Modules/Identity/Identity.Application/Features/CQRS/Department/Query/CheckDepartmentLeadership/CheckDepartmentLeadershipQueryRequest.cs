using FlashMediator;

namespace Identity.Application.Features.CQRS.Department.Query.CheckDepartmentLeadership
{
    public record CheckDepartmentLeadershipQueryRequest : IRequest<CheckDepartmentLeadershipQueryResponse>
    {
        public Guid UserId { get; init; }
    }
}
