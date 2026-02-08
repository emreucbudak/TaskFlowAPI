namespace Chat.Application.ChatNotification
{
    public interface IChatNotificationService
    {
        Task SendMessageToGroupAsync(Guid groupId, object message);
        Task SendMessageToUserAsync(Guid userId, object message);

    }
}
