using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Domain.Entities;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Create
{
    public class CreateCompanyPlanCommandRequest : IRequest
    {
        public string PlanName { get; set; }
        public PlanProperties PlanProperties { get; set; }
    }
}
