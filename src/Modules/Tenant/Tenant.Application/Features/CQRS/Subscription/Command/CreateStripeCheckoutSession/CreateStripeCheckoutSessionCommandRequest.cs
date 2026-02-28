using FlashMediator;

namespace Tenant.Application.Features.CQRS.Subscription.Command.CreateStripeCheckoutSession
{
    public sealed class CreateStripeCheckoutSessionCommandRequest : IRequest<CreateStripeCheckoutSessionCommandResponse>
    {
        public Guid CompanyId { get; init; }
        public string? PlanSlug { get; init; }
        public string? PlanName { get; init; }
        public string? SuccessUrl { get; init; }
        public string? CancelUrl { get; init; }
    }
}
