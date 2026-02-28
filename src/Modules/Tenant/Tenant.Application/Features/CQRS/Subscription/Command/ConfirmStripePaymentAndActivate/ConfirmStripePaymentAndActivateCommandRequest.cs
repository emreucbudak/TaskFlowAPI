using FlashMediator;

namespace Tenant.Application.Features.CQRS.Subscription.Command.ConfirmStripePaymentAndActivate;

public sealed class ConfirmStripePaymentAndActivateCommandRequest : IRequest<ConfirmStripePaymentAndActivateCommandResponse>
{
    public Guid CompanyId { get; init; }
    public string? PlanSlug { get; init; }
    public string? PlanName { get; init; }
    public string SessionId { get; init; } = string.Empty;
}
