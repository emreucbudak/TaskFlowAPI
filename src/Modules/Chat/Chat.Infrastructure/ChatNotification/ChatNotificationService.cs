using Chat.Application.ChatNotification;

namespace Chat.Infrastructure.ChatNotification
{
    public class ChatNotificationService : IChatNotificationService
    {
        public Task SendMessageToGroupAsync(Guid groupId, object message)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageToUserAsync(Guid userId, object message)
        {
            throw new NotImplementedException();
        }
    }
}
