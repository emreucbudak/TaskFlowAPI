namespace Identity.Application.Features.CQRS.Company.Queries.GetAll
{
    public record GetAllCompaniesQueriesResponse 
    {
        public class GetAllCompaniesQueryResponse
        {
            public List<CompanyDto> Companies { get; init; }
            public int TotalCount { get; init; }
            public int Page { get; init; }
            public int PageSize { get; init; }
        }


        public class CompanyDto
        {
            public Guid Id { get; init; }
            public string Name { get; init; }
        }
    }
}
