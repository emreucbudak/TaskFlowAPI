using TaskFlow.BuildingBlocks.Common;

namespace Stats.Domain.Entities
{
    public class WorkerStats : BaseEntity
    {
        public Guid UserId { get; private set; }
        public DateOnly Period { get; private set; }

        public int TotalTasksAssigned { get; private set; }
        public int TotalTasksCompleted { get; private set; }
        public int TasksCompletedBeforeDeadline { get; private set; }
        public int OverdueIncompleteTasksCount { get; private set; }

        private WorkerStats()
        {
        }

        public WorkerStats(Guid userId, DateOnly period)
        {
            UserId = userId;
            Period = new DateOnly(period.Year, period.Month, 1);
            TotalTasksAssigned = 0;
            TotalTasksCompleted = 0;
            TasksCompletedBeforeDeadline = 0;
            OverdueIncompleteTasksCount = 0;
        }

        public void RecordTaskAssigned()
        {
            TotalTasksAssigned++;
        }

        public void RecordTaskCompleted(bool wasBeforeDeadline)
        {
            TotalTasksCompleted++;
            if (wasBeforeDeadline)
            {
                TasksCompletedBeforeDeadline++;
            }
        }

        public void RecordTaskBecameOverdue()
        {
            OverdueIncompleteTasksCount++;
        }

        public void RecordOverdueTaskCompleted()
        {
            TotalTasksCompleted++;
            if (OverdueIncompleteTasksCount > 0)
            {
                OverdueIncompleteTasksCount--;
            }
        }
    }
}
