using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll
{
    public class GetAllCompanyPlanQueriesRequest : IRequest<List<GetAllCompanyPlanQueriesResponse>>, ICacheableQuery
    {
        public string CacheKey => "getallcompanyplan";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(30);
    }
}
