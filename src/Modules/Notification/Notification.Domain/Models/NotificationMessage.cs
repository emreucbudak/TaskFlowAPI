using TaskFlow.BuildingBlocks.Common;

namespace Notification.Domain.Models
{
    public class NotificationMessage : BaseEntity
    {
        public NotificationMessage(string title, string description, DateTime sendTime, bool ısRead)
        {
            Title = title;
            Description = description;
            SendTime = sendTime;
            IsRead = ısRead;
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime SendTime { get; private set; }
        public bool IsRead { get; private set; }
    }
}
