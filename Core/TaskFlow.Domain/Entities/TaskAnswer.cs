using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class TaskAnswer : BaseEntity
    {
        public string AnswerText { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }

    }
}
