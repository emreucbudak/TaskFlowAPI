namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}