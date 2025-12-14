namespace ProjectManagement.Domain.Entities
{
    public class Subtask
    {
        public string Description { get; set; }
        public int TaskStatusId { get; set; }
        public ICollection<SubTaskAnswer> Answers { get; set; }
    }
}
