namespace Identity.Application.Features.CQRS.Department.Query.GetAll
{
    public sealed record GetAllCompanyDepartmentsQueriesResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
