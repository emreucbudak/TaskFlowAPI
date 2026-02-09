using Chat.Application.Services;

namespace Chat.Application.Services
{
    public class GroupValidationService : IGroupValidationService
    {
        public Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            
            return Task.FromResult(true);
        }
    }
}