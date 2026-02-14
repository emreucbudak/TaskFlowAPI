namespace Notification.Application.IntegrationEvents
{
    public record IndividualTaskUpdatedIntegrationEvent(
            Guid Id,
            Guid AssignedUserId,
            string TaskTitle,
            string Description
        );
}
