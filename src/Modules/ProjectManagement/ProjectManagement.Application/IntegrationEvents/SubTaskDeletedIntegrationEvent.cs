namespace ProjectManagement.Application.IntegrationEvents
{
    public record SubTaskDeletedIntegrationEvent(Guid TaskId, Guid SubTaskId,Guid ReceiverUserId);
}
