using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Update
{
    public record UpdateIndividualTaskCommandRequest(
        Guid Id,
        string TaskTitle,
        string Description,
        DateOnly Deadline,
        int? TaskPriorityCategoryId = null) : IRequest;
}
