namespace ProjectManagement.Application.IntegrationEvents
{
    public record SubTaskUpdatedIntegrationEvent(Guid TaskId, Guid SubTaskId, string TaskTitle, string Description);
}
