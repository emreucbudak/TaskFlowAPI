using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chat.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHubs : Hub
    {
        private string? CurrentUserId => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        private string? CurrentUserName => Context.User?.Identity?.Name;

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string targetUserId, string message)
        {
            var senderId = CurrentUserId;
            await Clients.User(targetUserId).SendAsync("ReceivePrivateMessage", senderId, message, DateTime.UtcNow);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            var senderId = CurrentUserId;
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName, senderId, message, DateTime.UtcNow);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserJoinedGroup", groupName, CurrentUserId);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserLeftGroup", groupName, CurrentUserId);
        }

        public async Task UpdateMessage(string messageId, string newContent, string? groupName, string? targetUserId)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Clients.Group(groupName).SendAsync("ReceiveMessageUpdate", messageId, newContent);
            }
            else if (!string.IsNullOrEmpty(targetUserId))
            {
                await Clients.User(targetUserId).SendAsync("ReceiveMessageUpdate", messageId, newContent);
            }
        }

        public async Task UserTyping(string targetUserId, bool isTyping)
        {
            await Clients.User(targetUserId).SendAsync("UserIsTyping", CurrentUserId, isTyping);
        }

        public async Task UserTypingInGroup(string groupName, bool isTyping)
        {
            await Clients.OthersInGroup(groupName).SendAsync("UserIsTypingInGroup", groupName, CurrentUserId, isTyping);
        }

        public async Task MessageRead(string messageId, string? groupName, string? targetUserId)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Clients.Group(groupName).SendAsync("MessageReadNotification", messageId, CurrentUserId, groupName, DateTime.UtcNow);
            }
            else if (!string.IsNullOrEmpty(targetUserId))
            {
                await Clients.User(targetUserId).SendAsync("MessageReadNotification", messageId, CurrentUserId, DateTime.UtcNow);
            }
        }
    }
}