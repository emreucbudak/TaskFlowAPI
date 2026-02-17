using TaskFlow.BuildingBlocks.Enums;

namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ILimitedQueryable
    {
        Guid TenantId { get; }
        LimitType limitType { get; }



    }
}
