using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll
{
    public record GetAllSubTaskAnswerQueriesRequest : IRequest<List<GetAllSubTaskAnswerQueriesResponse>> ,ICacheableQuery
    {
        public Guid TaskId { get; init; }
        public Guid SubTaskId   { get; init; }

        public string CacheKey => "getallsubtaskanswerfromsubtask";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
