using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public record GetAllReportsQueryRequest : IRequest<PagedResult<GetAllReportsQueryResponse>>, ICacheableQuery
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string CacheKey => "getallreports";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
