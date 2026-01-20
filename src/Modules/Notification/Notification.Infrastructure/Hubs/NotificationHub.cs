using Microsoft.AspNetCore.SignalR;

namespace Notification.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
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
