namespace Identity.Application.Features.CQRS.Department.Query.CheckDepartmentLeadership
{
    public record CheckDepartmentLeadershipQueryResponse
    {
        public bool IsDepartmentLeader { get; init; }
        public Guid? DepartmentId { get; init; }
    }
}
