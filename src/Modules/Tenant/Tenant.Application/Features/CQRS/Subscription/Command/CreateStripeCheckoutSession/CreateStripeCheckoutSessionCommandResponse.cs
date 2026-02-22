namespace Tenant.Application.Features.CQRS.Subscription.Command.CreateStripeCheckoutSession
{
    public sealed record CreateStripeCheckoutSessionCommandResponse
    {
        public string? SessionId { get; init; }
        public string CheckoutUrl { get; init; } = string.Empty;
        public string PlanName { get; init; } = string.Empty;
        public int PlanPrice { get; init; }
    }
}
