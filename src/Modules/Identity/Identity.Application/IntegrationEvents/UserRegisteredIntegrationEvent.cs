namespace Identity.Application.IntegrationEvents;

public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Email,
    string Name,
    Guid TenantId);
