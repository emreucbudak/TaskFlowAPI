namespace TaskFlow.BuildingBlocks.Contracts.EventBus.Messages
{
    public record NotifyUserIntegrationEvent(Guid UserId, string Content);
}
