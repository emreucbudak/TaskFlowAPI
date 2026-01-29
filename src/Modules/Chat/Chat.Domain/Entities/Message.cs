using TaskFlow.BuildingBlocks.Common;

namespace Chat.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime SendTime { get; private set; }
        public Guid SenderId { get; private set; }
        public Guid? ReceiverId { get; private set; }
        public Guid? GroupId { get; private set; }
        public bool isDeleted { get; private set; }

        public Message(string content, bool ısRead, DateTime sendTime, Guid senderId, Guid receiverId, bool isDeleted, Guid? groupId)
        {
            Content = content;
            IsRead = ısRead;
            SendTime = sendTime;
            SenderId = senderId;
            ReceiverId = receiverId;
            this.isDeleted = isDeleted;
            GroupId = groupId;
        }

        public Message()
        {
        }
        public void UpdateContent(string newContent)
        {
            Content = newContent;
        }
        public void MarkAsRead()
        {
            IsRead = true;
        }
        public void MarkAsDeleted()
        {
            isDeleted = true;
        }


    }
}
