using FlashMediator;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandRequest : IRequest
    {
        public Guid CompanyPlanId { get; init; }
    }
}
