using FlashMediator;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll
{
    public class GetAllCompanyPlanQueriesHandler : IRequestHandler<GetAllCompanyPlanQueriesRequest, List<GetAllCompanyPlanQueriesResponse>>
    {
        private readonly ITenantReadRepository tenantReadRepository;

        public GetAllCompanyPlanQueriesHandler(ITenantReadRepository tenantReadRepository)
        {
            this.tenantReadRepository = tenantReadRepository;
        }

        public async Task<List<GetAllCompanyPlanQueriesResponse>> Handle(GetAllCompanyPlanQueriesRequest request, CancellationToken cancellationToken)
        {
            var companyPlans = await tenantReadRepository.GetAllPlans(false);
            var response = companyPlans.Select(plan => new GetAllCompanyPlanQueriesResponse
            {
                PlanName = plan.PlanName,
                PlanPrice = plan.PlanPrice,
                PlanProperties = plan.PlanProperties
            }).ToList();
            return response;
        }
    }
}
