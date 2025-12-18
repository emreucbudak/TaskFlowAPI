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
        public void AddAnswer(string AnswerText,Guid sender)
        {
            var answer = new TaskAnswer(answerText:AnswerText,senderId:sender);
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
        public void AddSubTask (string description,Guid AssignedUserId,string Title)
        {
            if (TaskStatusId == 2)
            {
                throw new InvalidOperationException("TAMAMLANMIŞ GÖREVE SUBTASK EKLENEMEZ");
            }
            var subtask = new Subtask(description,AssignedUserId,1,Title);
            _subtask.Add(subtask);
        }
        public void RemoveSubTask (Guid taskId)
        {
            var subtask = _subtask.Where(x=> x.Id == taskId).FirstOrDefault();
            if (subtask is null)
            {
                throw new InvalidOperationException("Subtask bulunamadı!");
            }
            _subtask.Remove(subtask); 
        }
        public void UpdateTaskName (string? name)
        {
            if (name is null)
            {
                return;
            }
            this.TaskName = name;
        }
        public void UpdateTaskDescription(string? description)
        {
            if (description is null)
            {
                return; 
            }
            this.Description = description;
        }
        public void UpdateDeadlineTime (DateTime? deadlineTime)
        {
            if(deadlineTime.HasValue)
            {
                return;
            }
            if(deadlineTime.Value<DateTime.UtcNow)
            this.DeadlineTime = deadlineTime.Value; 
        }
        public Subtask GetSubtask (Guid taskId)
        {
            var subTask = _subtask.Where(x=> x.Id == taskId).FirstOrDefault();
            if (subTask is null)
            {
                throw new Exception("subtask bulunamadı!");
            }
            return subTask;

        }
        public List<SubTaskAnswer> GetAllSubTaskAnswer(Guid taskId)
        {
            var subTask = _subtask.Where(x => x.Id == taskId).FirstOrDefault();
            if (subTask is null)
            {
                throw new Exception("subtask bulunamadı!");
            }
            var subTaskAnswer = subTask.subTaskAnswers;
            return subTaskAnswer.ToList();

        }
        public void RemoveTaskAnswer (Guid AnswerId)
        {
            var taskAnswer = _answers.Where(x=> x.Id == AnswerId).FirstOrDefault();
            if (taskAnswer is null)
            {
                throw new Exception("task answer bulunamadı!");
            }
            _answers.Remove(taskAnswer);

        }
        public void UpdateTaskAnswer (Guid TaskId,string answerText)
        {
            var taskAnswer = _answers.Where(x => x.Id == TaskId).FirstOrDefault();
            if (taskAnswer is null)
            {
                throw new Exception("TASKANSWER BULUNAMADI!");
            }
           taskAnswer.UpdateAnswerText(answerText);
        }
  


    }
}
