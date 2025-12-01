using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public Guid TaskStatusId { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public ICollection<TaskAnswer> TaskAnswer { get; set; }
        public DateTime DeadlineTime { get; set; }
        public Guid? TaskPriorityCategoryId { get; set; }
        public TaskPriorityCategory? TaskPriority { get; set; }
    }
}
