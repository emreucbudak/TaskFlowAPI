namespace Notification.Application.IntegrationEvents
{
    public record IndividualTaskDeletedIntegrationEvent(Guid Id, Guid AssignedUserId);
}
