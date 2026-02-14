namespace Notification.Application.IntegrationEvents
{
    public record SubTaskCreatedIntegrationEvent(
            Guid TaskId,
            string TaskTitle,
            string Description,
            Guid AssignedUserId
        );
}
