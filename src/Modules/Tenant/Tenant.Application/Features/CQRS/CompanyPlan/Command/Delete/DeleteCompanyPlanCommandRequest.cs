using FlashMediator.src.FlashMediator.Contracts;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandRequest : IRequest
    {
        public Guid CompanyPlanId { get; init; }
    }
}
