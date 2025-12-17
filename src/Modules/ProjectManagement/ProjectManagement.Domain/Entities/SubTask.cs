using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Subtask : BaseEntity
    {
        public string Description { get; set; }

        public Subtask(string description)
        {
            Description = description;
        }
        private List<SubTaskAnswer> Answers { get; set; } = new();
        public IReadOnlyCollection<SubTaskAnswer> subTaskAnswers => Answers;
        public void AddAnswer(SubTaskAnswer answer)
        {
            Answers.Add(answer);
        }

    }
}
