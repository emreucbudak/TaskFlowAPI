namespace Identity.Application.Services
{
    public interface IGroupValidationService
    {
        Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId);
    }
}
