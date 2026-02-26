using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class IndividualTasks : BaseEntity
    {
        public IndividualTasks(
            Guid assignedUserId,
            string taskTitle,
            string description,
            DateOnly deadline,
            int? taskPriorityCategoryId = null)
        {
            AssignedUserId = assignedUserId;
            TaskTitle = taskTitle;
            Description = description;
            Deadline = deadline;
            TaskPriorityCategoryId = taskPriorityCategoryId;
        }

        public Guid AssignedUserId { get; private set; }
        public string TaskTitle { get; private set; }
        public string Description { get; private set; }
        public DateOnly Deadline { get; private set; }
        public int? TaskPriorityCategoryId { get; private set; }
        public TaskPriorityCategory? TaskPriority { get; private set; }

        public void Update(string taskTitle, string description, DateOnly deadline, int? taskPriorityCategoryId = null)
        {
            TaskTitle = taskTitle;
            Description = description;
            Deadline = deadline;
            TaskPriorityCategoryId = taskPriorityCategoryId;
        }

    }
}
