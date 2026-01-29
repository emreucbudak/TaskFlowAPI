using TaskFlow.BuildingBlocks.Common;

namespace Chat.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; private set; }

        public Message(string content)
        {
            Content = content;
        }

        public Message()
        {
        }
        public void UpdateContent(string newContent)
        {
            Content = newContent;
        }
    }
}
