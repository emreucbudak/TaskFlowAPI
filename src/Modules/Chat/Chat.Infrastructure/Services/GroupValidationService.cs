using Chat.Application.Services;

namespace Chat.Infrastructure.Services
{
    public class GroupValidationService : IGroupValidationService
    {
        public Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            
            return Task.FromResult(true);
        }
    }
}