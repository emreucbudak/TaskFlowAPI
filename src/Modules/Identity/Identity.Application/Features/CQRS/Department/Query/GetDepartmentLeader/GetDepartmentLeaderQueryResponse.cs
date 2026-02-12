namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public record GetDepartmentLeaderQueryResponse
    {
        public Guid? LeaderId { get; init; }
    }
}
