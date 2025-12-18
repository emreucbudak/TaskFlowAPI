using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Task : BaseEntity
    {
        public Task(string taskName, string description, DateTime deadlineTime)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                throw new ArgumentException("Task Adı boş olamaz");
            }
            if (deadlineTime< DateTime.Now)
            {
                throw new ArgumentException("Bitiş tarihi şuandan önce (geçmiş tarih olamaz)");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Açıklama boş olamaz");
            }
            TaskName = taskName;
            Description = description;
            TaskStatusId = 1;
            DeadlineTime = deadlineTime;
        }

        public string TaskName { get; private set; }
        public string Description { get; private set; }
        public int TaskStatusId { get; private set; }
        public TaskStatus TaskStatus { get; private set; }
        private readonly List<TaskAnswer> _answers = new();
        public IReadOnlyCollection<TaskAnswer> taskAnswers => _answers; 
        private readonly List<Subtask> _subtask = new();
        public IReadOnlyCollection<Subtask> subtask => _subtask;
        public DateTime DeadlineTime { get; private set; }
        public int? TaskPriorityCategoryId { get; private set; }
        public TaskPriorityCategory? TaskPriority { get; private set; }
        public void AddAnswer(string AnswerText)
        {
            var answer = new TaskAnswer()
            {
                AnswerText = AnswerText
            };
            _answers.Add(answer);
        }
        public void UpdateTaskPriority (int taskPriority)
        {
            this.TaskPriorityCategoryId = taskPriority;
        }
        public void UpdateTaskStatus(int taskStatusId)
        {
            this.TaskStatusId = taskStatusId;
        }
        public void AddSubTask (string description)
        {
            if (TaskStatusId == 2)
            {
                throw new InvalidOperationException("TAMAMLANMIŞ GÖREVE SUBTASK EKLENEMEZ");
            }
            var subtask = new Subtask(description);
            _subtask.Add(subtask);
        }
  


    }
}
