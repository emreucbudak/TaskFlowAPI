using TaskFlow.BuildingBlocks.Common;

namespace Notification.Domain.Models
{
    public class NotificationMessage : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SendTime { get; set; }
    }
}
