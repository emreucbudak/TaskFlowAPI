namespace ProjectManagement.Application.IntegrationEvents;

public sealed record TaskCreatedIntegrationEvent(
    Guid TaskId,
    string TaskName,
    string TaskDescription,
    DateOnly DeadlineTime);
