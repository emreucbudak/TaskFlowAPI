namespace TaskFlow.BuildingBlocks.Contracts.EventBus.Messages
{
    public record ReportCreatedIntegrationEvent(Guid ReportId, string Content, Guid NotifiedDepartmentId);
}
