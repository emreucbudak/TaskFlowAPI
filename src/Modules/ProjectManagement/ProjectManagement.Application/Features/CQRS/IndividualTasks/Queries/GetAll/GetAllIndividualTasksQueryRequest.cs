using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetAll
{
    public record GetAllIndividualTasksQueryRequest(int PageNumber, int PageSize) : IRequest<List<GetAllIndividualTasksQueryResponse>>;
}
