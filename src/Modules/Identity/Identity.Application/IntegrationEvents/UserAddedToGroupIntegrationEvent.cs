namespace Identity.Application.IntegrationEvents
{
    public record UserAddedToGroupIntegrationEvent(Guid GroupId, Guid UserId, int RolesId);
}
