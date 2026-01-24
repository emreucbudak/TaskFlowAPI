using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll
{
    public class GetAllCompanyPlanQueriesHandler : IRequestHandler<GetAllCompanyPlanQueriesRequest, List<GetAllCompanyPlanQueriesResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITenantReadRepository tenantReadRepository;

        public GetAllCompanyPlanQueriesHandler(IUnitOfWork unitOfWork, ITenantReadRepository tenantReadRepository)
        {
            this.unitOfWork = unitOfWork;
            this.tenantReadRepository = tenantReadRepository;
        }

        public async Task<List<GetAllCompanyPlanQueriesResponse>> Handle(GetAllCompanyPlanQueriesRequest request, CancellationToken cancellationToken)
        {
            var companyPlans = await tenantReadRepository.GetAllPlans(false);
            var response = companyPlans.Select(plan => new GetAllCompanyPlanQueriesResponse
            {
                PlanName = plan.PlanName,
                PlanProperties = plan.PlanProperties
            }).ToList();
            return response;
        }
    }
}
