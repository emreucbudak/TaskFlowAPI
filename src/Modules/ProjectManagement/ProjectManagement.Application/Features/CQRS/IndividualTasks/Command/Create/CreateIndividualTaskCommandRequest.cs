using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create
{
    public record CreateIndividualTaskCommandRequest(Guid AssignedUserId, string TaskTitle, string Description, DateOnly Deadline) : IRequest;
}
