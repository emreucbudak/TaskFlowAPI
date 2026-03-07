namespace Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers;

public sealed record GetAllCompanyUsersQueriesResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<UserDepartmentMembershipResponse> DepartmentMemberships { get; init; } = [];
}

public sealed record UserDepartmentMembershipResponse
{
    public Guid DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public int DepartmentRoleId { get; init; }
}
