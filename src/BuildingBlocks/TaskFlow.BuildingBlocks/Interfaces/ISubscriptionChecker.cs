namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ISubscriptionChecker
    {
        Task<bool> CheckSubscriptionStatusAsync(Guid tenantId);

    }
}
