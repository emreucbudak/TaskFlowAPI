using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById
{
    public record GetIndividualTaskByIdQueryRequest(Guid Id) : IRequest<GetIndividualTaskByIdQueryResponse>;
}
