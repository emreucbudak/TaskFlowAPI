namespace Chat.Application.Services
{
    public interface IGroupValidationService
    {
        /// <summary>
        /// Checks if the group exists, is active, and if the user is a member.
        /// </summary>
        /// <param name="userId">The user ID (Sender).</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>True if valid, False otherwise.</returns>
        Task<bool> ValidateGroupMembershipAsync(Guid userId, Guid groupId);
    }
}