namespace Notification.Domain.Entities
{
    public class NotificationMessage : TaskFlow.BuildingBlocks.Common.BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SendTime { get; set; }
    }
}
