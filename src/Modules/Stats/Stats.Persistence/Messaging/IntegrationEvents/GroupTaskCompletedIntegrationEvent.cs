namespace Stats.Persistence.Messaging.IntegrationEvents;

public sealed record GroupTaskCompletedIntegrationEvent(
    Guid TaskId,
    DateOnly Deadline,
    DateOnly CompletedOn,
    List<Guid> AssignedUserIds);
