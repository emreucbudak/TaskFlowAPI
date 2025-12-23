using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Subtask : BaseEntity
    {
        public string TaskTitle { get; private set; }
        public string Description { get; private set; }
        public Guid AssignedUserId {  get; private set; }
        public int TaskStatusId { get; private set; }
        public TaskStatus TaskStatus { get; private set; }

        public Subtask(string description, Guid assignedUserId, int taskStatusId, string taskTitle)
        {
            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                throw new Exception("Task title'ı boş veya null olamaz");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new Exception("Description boş veya null olamaz");
            }
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
        public void RemoveSubTaskAnswer (Guid AnswerId)
        {
            var answer = subTaskAnswers.Where(x=> x.Id == AnswerId).FirstOrDefault();
            if (answer is null)
            {
                throw new Exception("Subtaskın Answeri bulunamadı!");
            }
            Answers.Remove(answer);
        }
        public void UpdateSubTaskAnswer (string taskAnswer,Guid subTaskAnswerId)
        {
            var subTask = Answers.Where(x => x.Id == subTaskAnswerId).FirstOrDefault();
            if (subTask is null)
            {
                throw new Exception("subtakanswer bulunamadı!");
            }
            subTask.UpdateAnswerText(taskAnswer);

        }
        public string GetTaskStatus()
        {
            return TaskStatus.StatusName;
        }

    }
}
