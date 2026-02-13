namespace Notification.Application.IntegrationEvents
{
    public record NotifyUserIntegrationEvent(Guid UserId, string Content);
}
