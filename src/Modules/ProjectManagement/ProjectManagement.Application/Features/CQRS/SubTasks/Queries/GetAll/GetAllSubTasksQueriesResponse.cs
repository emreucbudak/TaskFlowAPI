namespace ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll
{
    public record GetAllSubTasksQueriesResponse
    {
        public string TaskTitle { get; init; }
        public string Description { get; init; }
        public Guid AssignedUserId { get; init; }
        public int TaskStatusId { get; init; }
    }
}
