namespace Stats.Application.Features.CQRS.WorkerStats.Queries.GetAll
{
    public record GetAllWorkersStatsByPeriodQueryResponse(
        Guid Id,
        Guid UserId,
        DateOnly Period,
        int TotalTasksAssigned,
        int TotalTasksCompleted,
        int TasksCompletedBeforeDeadline,
        int OverdueIncompleteTasksCount);
}
