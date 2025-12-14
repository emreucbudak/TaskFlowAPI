namespace ProjectManagement.Domain.Entities
{
    public class SubTaskAnswer
    {
        public string AnswerText { get; set; }
        public Guid SubTaskId { get; set; }
        public ProjectManagement.Domain.Entities.Subtask SubTask { get; set; }
    }
}
