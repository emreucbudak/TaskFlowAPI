

using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll
{
    public record GetAllSubTasksQueriesRequest :IRequest<List<GetAllSubTasksQueriesResponse>> , ICacheableQuery
    {
        public Guid TaskId { get; init; }

        public string CacheKey => "getallsubtaskfromtask";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
