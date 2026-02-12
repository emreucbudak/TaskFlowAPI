using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Queries.GetAll
{
    public class GetAllCompanyGroupsQueriesRequest : IRequest<List<GetAllCompanyGroupsQueriesResponse>>, ICacheableQuery
    {
        public Guid CompanyId { get; init; }

        public string CacheKey => "getallcompanygroups";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
