namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ISubscriptionLimitCheckerService
    {
        Task CheckUserLimitAsync(Guid tenantId);
        Task CheckProjectLimitAsync(Guid tenantId);
        Task CheckStorageLimitAsync(Guid tenantId, long fileSizeInBytes);
    }
}