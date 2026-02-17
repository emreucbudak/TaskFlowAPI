namespace Identity.Application.IntegrationEvents;

public sealed record GroupChatCreatedIntegrationEvent(
    Guid GroupChatId,
    string GroupName,
    Guid TenantId);
