using TaskFlow.BuildingBlocks.Exceptions;

namespace Tenant.Application.Features.CQRS.Subscription.Exceptions
{
    public sealed class CompanyPlanNotFoundExceptions : NotFoundExceptions
    {
        public CompanyPlanNotFoundExceptions() : base("Plan bulunamadi.")
        {
        }
    }
}
