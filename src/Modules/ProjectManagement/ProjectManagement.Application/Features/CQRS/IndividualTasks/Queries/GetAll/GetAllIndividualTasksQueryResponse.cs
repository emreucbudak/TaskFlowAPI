namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetAll
{
    public record GetAllIndividualTasksQueryResponse(Guid Id, Guid AssignedUserId, string TaskTitle, string Description, DateOnly Deadline);
}
