namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId
{
    public record GetIndividualTasksByUserIdQueryResponse(Guid Id, Guid AssignedUserId, string TaskTitle, string Description, DateOnly Deadline);
}
