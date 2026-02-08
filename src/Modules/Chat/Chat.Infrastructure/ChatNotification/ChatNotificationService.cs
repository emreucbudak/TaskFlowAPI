using Chat.Application.ChatNotification;
using Chat.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Infrastructure.ChatNotification
{
    public class ChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHubs> _hubContext;

        public ChatNotificationService(IHubContext<ChatHubs> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToGroupAsync(Guid groupId, object message)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", groupId.ToString(), message);
        }

        public async Task SendMessageToUserAsync(Guid userId, object message)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceivePrivateMessage", userId.ToString(), message);
        }
    }
}
