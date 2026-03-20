using FlashMediator;
using Identity.Application.Repositories;

namespace Identity.Application.Features.CQRS.Department.Query.CheckDepartmentLeadership
{
    public class CheckDepartmentLeadershipQueryHandler(
        IDepartmentMemberRepository departmentMemberRepository)
        : IRequestHandler<CheckDepartmentLeadershipQueryRequest, CheckDepartmentLeadershipQueryResponse>
    {
        public async Task<CheckDepartmentLeadershipQueryResponse> Handle(
            CheckDepartmentLeadershipQueryRequest request,
            CancellationToken cancellationToken)
        {
            var leaderMembership = await departmentMemberRepository
                .GetLeaderMembershipAsync(request.UserId, cancellationToken);

            return new CheckDepartmentLeadershipQueryResponse
            {
                IsDepartmentLeader = leaderMembership is not null,
                DepartmentId = leaderMembership?.DepartmentId
            };
        }
    }
}
