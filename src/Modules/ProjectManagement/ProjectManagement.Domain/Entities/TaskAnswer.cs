using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class TaskAnswer : BaseEntity
    {
        public string AnswerText { get; set; }
        public Guid TaskId { get; set; }
        public Task Task { get; set; }

    }
}
