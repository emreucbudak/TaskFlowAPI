using Microsoft.AspNetCore.SignalR;

namespace Chat.Infrastructure.Hubs
{
    public class ChatHubs : Hub
    {

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.UtcNow);
        }


        public async Task SendPrivateMessage(string targetConnectionId, string user, string message)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceivePrivateMessage", user, message, DateTime.UtcNow);
        }


        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName, user, message, DateTime.UtcNow);
        }


        public async Task JoinGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserJoinedGroup", groupName, userName);
        }

        public async Task LeaveGroup(string groupName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserLeftGroup", groupName, userName);
        }


        public async Task UserTyping(string user, bool isTyping)
        {
            await Clients.Others.SendAsync("UserIsTyping", user, isTyping);
        }


        public async Task UserTypingInGroup(string groupName, string user, bool isTyping)
        {
            await Clients.OthersInGroup(groupName).SendAsync("UserIsTypingInGroup", groupName, user, isTyping);
        }


        public async Task MessageRead(string messageId, string userId)
        {
            await Clients.All.SendAsync("MessageReadNotification", messageId, userId, DateTime.UtcNow);
        }


        public async Task BroadcastOnlineCount(int count)
        {
            await Clients.All.SendAsync("OnlineUsersCount", count);
        }
    }
}