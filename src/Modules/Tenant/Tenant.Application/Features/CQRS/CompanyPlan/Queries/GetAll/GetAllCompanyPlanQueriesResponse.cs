using Tenant.Domain.Entities;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll
{
    public record GetAllCompanyPlanQueriesResponse 
    {
        public string PlanName { get; init; }
        public PlanProperties PlanProperties { get; init; }
    }
}
