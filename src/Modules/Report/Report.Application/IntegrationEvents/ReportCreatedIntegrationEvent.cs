namespace Report.Application.IntegrationEvents
{
    public record ReportCreatedIntegrationEvent(Guid ReportId, string Content, Guid NotifiedDepartmentId);
}
