namespace Identity.Application.IntegrationEvents
{
    public record UserRemovedFromDepartmentIntegrationEvent(Guid UserId, Guid DepartmentId);
}
