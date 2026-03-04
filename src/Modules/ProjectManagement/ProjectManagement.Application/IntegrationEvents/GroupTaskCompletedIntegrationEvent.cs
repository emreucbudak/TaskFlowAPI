namespace ProjectManagement.Application.IntegrationEvents;

public sealed record GroupTaskCompletedIntegrationEvent(
    Guid TaskId,
    DateOnly Deadline,
    DateOnly CompletedOn,
    List<Guid> AssignedUserIds);
