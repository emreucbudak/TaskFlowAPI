namespace Identity.Application.IntegrationEvents
{
    public record UserRemovedFromGroupIntegrationEvent(Guid GroupId, Guid UserId);
}
