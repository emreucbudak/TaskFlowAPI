namespace Identity.Application.IntegrationEvents
{
    public record UserAddedToDepartmentIntegrationEvent(Guid UserId, Guid DepartmentId);
}
