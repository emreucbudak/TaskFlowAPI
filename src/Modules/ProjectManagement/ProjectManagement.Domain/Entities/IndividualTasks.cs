using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class IndividualTasks : BaseEntity
    {
        public Guid AssignedUserId { get; private set; }
        public string TaskTitle { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateOnly Deadline { get; private set; }

    }
}
