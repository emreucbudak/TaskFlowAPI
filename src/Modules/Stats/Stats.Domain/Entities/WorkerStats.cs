using TaskFlow.BuildingBlocks.Common;

namespace Stats.Domain.Entities
{
    public class WorkerStats : BaseEntity
    {
        private const int TaskCompletionBasePoints = 10;
        private const int EarlyCompletionDailyBonusPoints = 10;

        public Guid UserId { get; private set; }
        public DateOnly Period { get; private set; }

        public int TotalTasksAssigned { get; private set; }
        public int TotalTasksCompleted { get; private set; }
        public int TasksCompletedBeforeDeadline { get; private set; }
        public int OverdueIncompleteTasksCount { get; private set; }
        public int TotalPoints { get; private set; }

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
            TotalPoints = 0;
        }

        public void RecordTaskAssigned()
        {
            TotalTasksAssigned++;
        }

        public void RecordTaskCompleted(bool wasBeforeDeadline)
        {
            TotalTasksCompleted++;
            TotalPoints += TaskCompletionBasePoints;

            if (wasBeforeDeadline)
            {
                TasksCompletedBeforeDeadline++;
                TotalPoints += EarlyCompletionDailyBonusPoints;
            }
        }

        public void RecordTaskCompleted(DateOnly completedOn, DateOnly deadline)
        {
            TotalTasksCompleted++;

            var earlyCompletedDayCount = deadline.DayNumber - completedOn.DayNumber;
            if (earlyCompletedDayCount > 0)
            {
                TasksCompletedBeforeDeadline++;
            }
            else
            {
                earlyCompletedDayCount = 0;
            }

            TotalPoints += TaskCompletionBasePoints + (earlyCompletedDayCount * EarlyCompletionDailyBonusPoints);
        }

        public void RecordTaskBecameOverdue()
        {
            OverdueIncompleteTasksCount++;
        }

        public void RecordOverdueTaskCompleted()
        {
            TotalTasksCompleted++;
            TotalPoints += TaskCompletionBasePoints;

            if (OverdueIncompleteTasksCount > 0)
            {
                OverdueIncompleteTasksCount--;
            }
        }
    }
}
