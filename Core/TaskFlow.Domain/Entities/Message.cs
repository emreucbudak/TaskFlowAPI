using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
