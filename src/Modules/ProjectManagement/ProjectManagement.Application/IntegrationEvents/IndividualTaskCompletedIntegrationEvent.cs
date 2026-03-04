namespace ProjectManagement.Application.IntegrationEvents;

public sealed record IndividualTaskCompletedIntegrationEvent(
    Guid TaskId,
    Guid AssignedUserId,
    DateOnly Deadline,
    DateOnly CompletedOn);
