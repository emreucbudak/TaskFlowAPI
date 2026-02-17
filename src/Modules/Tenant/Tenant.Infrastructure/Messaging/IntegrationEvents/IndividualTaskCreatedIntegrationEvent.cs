namespace Tenant.Infrastructure.Messaging.IntegrationEvents;

public sealed record IndividualTaskCreatedIntegrationEvent(
    Guid TaskId,
    Guid AssignedUserId,
    string TaskTitle,
    string TaskDescription,
    DateOnly Deadline,
    Guid TenantId);
