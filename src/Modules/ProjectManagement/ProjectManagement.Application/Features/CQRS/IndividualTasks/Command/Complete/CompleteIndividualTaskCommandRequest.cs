using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Complete
{
    public record CompleteIndividualTaskCommandRequest(Guid Id) : IRequest;
}
