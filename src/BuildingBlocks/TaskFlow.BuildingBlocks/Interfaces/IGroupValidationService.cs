namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface IGroupValidationService
    {

        Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId);
    }
}