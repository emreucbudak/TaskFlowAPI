using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Task : BaseEntity
    {
        public Task(string taskName, string description, DateOnly deadlineTime, DateOnly createdDate)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                throw new ArgumentException("Task Adı boş olamaz");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Açıklama boş olamaz");
            }
            TaskName = taskName;
            Description = description;
            TaskStatusId = 1;
            DeadlineTime = deadlineTime;
            CreatedDate = createdDate;

        }

        public string TaskName { get; private set; }
        public string Description { get; private set; }
        public int TaskStatusId { get; private set; }
        public TaskStatus TaskStatus { get; private set; }
        private readonly List<Subtask> _subtask = new();
        public IReadOnlyCollection<Subtask> subtask => _subtask;
        public DateOnly DeadlineTime { get; private set; }
        public int? TaskPriorityCategoryId { get; private set; }
        public TaskPriorityCategory? TaskPriority { get; private set; }
        public DateOnly CreatedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public void UpdateTaskPriority (int taskPriority)
        {
            this.TaskPriorityCategoryId = taskPriority;
        }
        public void UpdateTaskStatus(int taskStatusId)
        {
            this.TaskStatusId = taskStatusId;
        }
        public Subtask AddSubTask (string description,Guid AssignedUserId,string Title,Guid taskId)
        {
            if (TaskStatusId == 2)
            {
                throw new InvalidOperationException("TAMAMLANMIŞ GÖREVE SUBTASK EKLENEMEZ");
            }
            var subtask = new Subtask(description,AssignedUserId,1,Title,taskId);
            _subtask.Add(subtask);
            return subtask;
        }
        public void RemoveSubTask (Guid taskId)
        {
            var subtask = _subtask.Where(x=> x.Id == taskId).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(subtask);
            _subtask.Remove(subtask); 
        }
        public void UpdateTaskName (string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Task adı boş olamaz");
            this.TaskName = name;
        }
        public void UpdateTaskDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new Exception("Task açıklaması boş olamaz");
            this.Description = description;
        }
        public void UpdateDeadlineTime (DateOnly? deadlineTime)
        {
            if(!deadlineTime.HasValue)
            {
                return;
            }
            if (deadlineTime.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new Exception("deadline zamanı şimdiki zamandan geride olamaz");
            }
            this.DeadlineTime = deadlineTime.Value; 
        }
        public Subtask GetSubtask (Guid taskId)
        {
            var subTask = _subtask.Where(x=> x.Id == taskId).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(subTask);
            return subTask;

        }
        public List<SubTaskAnswer> GetAllSubTaskAnswer(Guid taskId)
        {
            var subTask = _subtask.Where(x => x.Id == taskId).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(subTask);
            var subTaskAnswer = subTask.subTaskAnswers;
            return subTaskAnswer.ToList();

        }
        public List<Subtask> GetAllSubTasks()
        {
            return subtask.ToList();
        }
        public string GetTaskStatus()
        {
            return TaskStatus?.StatusName ?? string.Empty;
        }
        public string GetTaskPriorityCategory()
        {
            return TaskPriority?.CategoryName ?? string.Empty;
        }
  


    }
}
