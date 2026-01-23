using Microsoft.AspNetCore.SignalR;
using Notification.Application;
using Notification.Application.Repositories;
using Notification.Domain.Models;
using Notification.Infrastructure.Hubs;

namespace Notification.Infrastructure.NotificationService
{
    public class UserNotificationService(IHubContext<NotificationHub> hubContext,INotificationWriteRepository rp) : INotificationService
    {
        public async Task SendNotificationToUserAsync(string userId, NotificationMessage nm,string? groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await hubContext.Clients.Group(groupName).SendAsync("NewMessage", nm.Title, nm.Description);
            }
            else
            {
                await hubContext.Clients.User(userId).SendAsync("NewMessage", nm.Title, nm.Description);
            }
            await rp.SendNotification(nm);

        }
    }
}
