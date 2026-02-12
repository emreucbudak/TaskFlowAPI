using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Company.Queries.GetAll
{
    public record GetAllCompaniesQueriesRequest : IRequest<PagedResult<GetAllCompaniesQueriesResponse>>, ICacheableQuery
    {
        public GetAllCompaniesQueriesRequest(int? pageNumber, int? pageSize)
        {
            PageNumber = pageNumber ?? 1;
            PageSize = pageSize ?? 50;
        }

        public int PageNumber { get; init; } 
        public int PageSize { get; init; }

        public string CacheKey => "all_companies";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(30);
    }
}
