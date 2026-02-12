namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetByUserAndPeriod
{
    public record GetWorkerStatsByUserAndPeriodQueryResponse(
        Guid Id,
        Guid UserId,
        DateOnly Period,
        int TotalTasksAssigned,
        int TotalTasksCompleted,
        int TasksCompletedBeforeDeadline,
        int OverdueIncompleteTasksCount);
}
