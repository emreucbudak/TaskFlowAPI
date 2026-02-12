using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public record GetAllTasksQueriesRequest : IRequest<PagedResult<GetAllTasksQueriesResponse>>, ICacheableQuery
    {
        public GetAllTasksQueriesRequest(Guid companyId, int pageNumber, int pageSize)
        {
            CompanyId = companyId;
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
        }

        public Guid CompanyId { get; init; }
        public int pageNumber { get; init; }
        public int pageSize { get; init; }

        public string CacheKey => "getalltasks";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
