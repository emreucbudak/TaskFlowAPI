namespace Stats.Persistence.Messaging.IntegrationEvents;

public sealed record IndividualTaskCompletedIntegrationEvent(
    Guid TaskId,
    Guid AssignedUserId,
    DateOnly Deadline,
    DateOnly CompletedOn);
