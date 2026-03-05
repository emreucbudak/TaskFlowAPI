using TaskFlow.BuildingBlocks.Common;

namespace Notification.Domain.Models
{
    public class NotificationMessage : BaseEntity
    {
        public NotificationMessage(string title, string description, DateTime sendTime, bool isRead, Guid receiverUserId)
        {
            if (receiverUserId == Guid.Empty)
            {
                throw new ArgumentException("Receiver user id cannot be empty.", nameof(receiverUserId));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            Title = title;
            Description = description;
            SendTime = sendTime;
            IsRead = isRead;
            ReceiverUserId = receiverUserId;
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime SendTime { get; private set; }
        public bool IsRead { get; private set; }
        public Guid ReceiverUserId { get; private set; }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
