namespace Tenant.Infrastructure.Messaging.IntegrationEvents;

public sealed record GroupChatCreatedIntegrationEvent(
    Guid GroupChatId,
    string GroupName,
    Guid TenantId);
