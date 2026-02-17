using TaskFlow.BuildingBlocks.Enums;

namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ISubscriptionLimitCheckerService
    {
        LimitType LimitType { get; }
        Task CheckLimitAsync(Guid companyId);

    }
}