using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class Subtask : BaseEntity
    {
        public string Description { get; set; }
        public int TaskStatusId { get; set; }
        public ICollection<SubTaskAnswer> Answers { get; set; }
    }
}
