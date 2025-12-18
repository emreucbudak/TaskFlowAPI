using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Subtask : BaseEntity
    {
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public Guid AssignedUserId {  get; set; }
        public int TaskStatusId { get; set; }
        public TaskStatus TaskStatus { get; set; }

        public Subtask(string description, Guid assignedUserId, int taskStatusId, string taskTitle)
        {
            Description = description;
            AssignedUserId = assignedUserId;
            TaskStatusId = taskStatusId;
            TaskTitle = taskTitle;
        }
        private List<SubTaskAnswer> Answers { get; set; } = new();
        public IReadOnlyCollection<SubTaskAnswer> subTaskAnswers => Answers;
        public void AddAnswer(SubTaskAnswer answer)
        {
            Answers.Add(answer);
        }
        public void UpdateTaskStatus(int taskStatus)
        {
            this.TaskStatusId = taskStatus;
        }
        public void UpdateTaskTitle(string taskTitle)
        {
            this.TaskTitle = taskTitle; 
        }
        public void UpdateTaskDescription (string taskDescription)
        {
            this.Description = taskDescription;
        }
    }
}
