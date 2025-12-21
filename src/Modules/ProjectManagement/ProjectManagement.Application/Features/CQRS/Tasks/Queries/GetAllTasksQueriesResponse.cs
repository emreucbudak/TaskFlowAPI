namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public record GetAllTasksQueriesResponse
    {
        public string TaskName { get; init; }
        public string Description { get; init; }
        public DateTime DeadlineTime { get; init; }
        public string StatusName { get; init; }
        public string CategoryName { get; init; }

    }
}
