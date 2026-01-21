using FlashMediator;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.AspNetCore.SignalR;
using TaskFlow.BuildingBlocks.Contracts.UserGroups;

namespace Notification.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IMediator mediator;

        public NotificationHub(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendNotificationToUser(Guid userId,string gorevBasligi,string message)
        {
            await Clients.User(userId.ToString()).SendAsync("NewMessage", message);
        }
        public async Task SendNotificationToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("NewMessage", message);
        }
        public async override  Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            List<string> groups = await mediator.Send(new GetUserAllGroupsNameQueriesRequest(Guid.Parse(userId)));
            foreach(var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
            }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
