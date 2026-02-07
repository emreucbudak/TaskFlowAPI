namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById
{
    public record GetIndividualTaskByIdQueryResponse(Guid Id, Guid AssignedUserId, string TaskTitle, string Description, DateOnly Deadline);
}
