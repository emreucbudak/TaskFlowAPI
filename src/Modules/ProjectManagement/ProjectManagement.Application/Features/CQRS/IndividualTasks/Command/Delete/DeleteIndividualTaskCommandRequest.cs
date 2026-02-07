using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Delete
{
    public record DeleteIndividualTaskCommandRequest(Guid Id) : IRequest;
}
