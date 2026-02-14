namespace Notification.Infrastructure.Messaging.Consumers
{
    public record SubTaskUpdatedIntegrationEvent(Guid TaskId, Guid SubTaskId, string TaskTitle, string Description, Guid ReceiverUserId);
}
