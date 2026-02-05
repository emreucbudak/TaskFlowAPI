using FlashMediator;
using TaskFlow.BuildingBlocks.Common;

namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public record GetUserAllNotificationsQueriesRequest : IRequest<PagedResult<GetUserAllNotificationsQueriesResponse>>
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
        }
}
