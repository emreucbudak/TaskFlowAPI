using FlashMediator;

namespace Identity.Application.Features.CQRS.Company.Queries.GetAll
{
    public record GetAllCompaniesQueriesRequest : IRequest<List<GetAllCompaniesQueriesResponse>>
    {
        public GetAllCompaniesQueriesRequest(int? pageNumber, int? pageSize)
        {
            PageNumber = pageNumber ?? 1;
            PageSize = pageSize ?? 50;
        }

        public int PageNumber { get; init; } 
        public int PageSize { get; init; }

    }
}
