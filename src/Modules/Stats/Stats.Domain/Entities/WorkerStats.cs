using TaskFlow.BuildingBlocks.Common;

namespace Stats.Domain.Entities
{
    public class WorkerStats : BaseEntity
    {
        public WorkerStats()
        {
        }

        public WorkerStats(Guid userId, Guid completedTaskId, DateOnly date)
        {
            UserId = userId;
            CompletedTaskId = completedTaskId;
            Date = date;
        }

        public Guid UserId { get; private set; }
        public Guid CompletedTaskId { get; private set; }
        public DateOnly Date { get; private set; }


    }
}
