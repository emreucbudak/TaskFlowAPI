namespace Notification.Domain.Models
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SendTime { get; set; }
    }
}
