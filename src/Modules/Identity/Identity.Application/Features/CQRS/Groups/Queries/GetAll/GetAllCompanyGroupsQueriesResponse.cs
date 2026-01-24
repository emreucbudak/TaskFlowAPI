namespace Identity.Application.Features.CQRS.Groups.Queries.GetAll
{
    public record GetAllCompanyGroupsQueriesResponse
    {
        public string GroupName { get; init; }
        public List<string> WorkerName { get; init; }
        public List<string> DepartmenName { get; init; }


    }
}
