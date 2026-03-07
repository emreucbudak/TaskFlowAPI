using FlashMediator;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetByAssignedUsers
{
    public sealed record GetGroupTasksByAssignedUsersQueryRequest : IRequest<PagedResult<GetAllTasksQueriesResponse>>, ICacheableQuery
    {
        public IReadOnlyCollection<Guid> AssignedUserIds { get; init; } = [];
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public string CacheKey
        {
            get
            {
                var normalizedUserIds = AssignedUserIds?
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .OrderBy(id => id)
                    .Select(id => id.ToString("N"))
                    .ToList() ?? [];

                var userIdsSegment = normalizedUserIds.Count == 0
                    ? "none"
                    : string.Join(",", normalizedUserIds);

                return $"getgrouptasks:{userIdsSegment}:{PageNumber}:{PageSize}";
            }
        }

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}