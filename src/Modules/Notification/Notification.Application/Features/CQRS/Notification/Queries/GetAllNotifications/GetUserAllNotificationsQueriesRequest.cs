using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public record GetUserAllNotificationsQueriesRequest : IRequest<PagedResult<GetUserAllNotificationsQueriesResponse>>, ICacheableQuery
    {
        public GetUserAllNotificationsQueriesRequest(Guid userId, int pageNumber, int pageSize)
        {
            this.userId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public Guid userId { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string CacheKey => "usernotifications";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(30);
    }
}
