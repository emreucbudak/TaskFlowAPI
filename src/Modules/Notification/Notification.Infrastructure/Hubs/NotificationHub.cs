using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.AspNetCore.SignalR;

namespace Notification.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToUser(Guid userId,string gorevBasligi,string message)
        {
            await Clients.User(userId.ToString()).SendAsync("NewMessage", message);
        }
        public async Task SendNotificationToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("NewMessage", message);
        }
        public override  Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
