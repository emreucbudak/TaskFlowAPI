namespace Notification.Application.IntegrationEvents
{
    public record IndividualTaskCreatedIntegrationEvent(
            Guid AssignedUserId,
            string TaskTitle,
            string TaskDescription,
            DateOnly Deadline
        );
}
