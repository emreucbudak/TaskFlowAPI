using FlashMediator;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    public record UpdateCompanyPlanCommandRequest : IRequest
    {
        public int PeopleAddedLimit { get; init; }
        public int TeamLimit { get; init; }
        public int IndividualTaskLimit { get; init; }
        public bool IsIncludeReporting { get; init; }
        public Guid CompanyPlanId { get; init; }
    }
}
