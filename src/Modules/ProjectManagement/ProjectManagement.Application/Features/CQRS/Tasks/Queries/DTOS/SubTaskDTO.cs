namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries.DTOS
{
    public record SubTaskDTO
    {
        public string TaskTitle { get; init; }
        public string Description { get; init; }
        public Guid AssignedUserId { get; init; }
        public string StatusName { get; init; }
    }
}
