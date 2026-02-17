namespace Tenant.Infrastructure.Messaging.IntegrationEvents;

public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Email,
    string Name,
    Guid TenantId);
