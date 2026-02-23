namespace Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers;

public sealed record GetAllCompanyUsersQueriesResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
