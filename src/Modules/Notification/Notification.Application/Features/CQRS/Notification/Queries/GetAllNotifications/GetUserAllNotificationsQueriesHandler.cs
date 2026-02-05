using FlashMediator;
using Notification.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public class GetUserAllNotificationsQueriesHandler : IRequestHandler<GetUserAllNotificationsQueriesRequest, PagedResult<GetUserAllNotificationsQueriesResponse>>
    {
        private readonly INotificationReadRepository _notificationReadRepository;

        public GetUserAllNotificationsQueriesHandler(INotificationReadRepository notificationReadRepository)
        {
            _notificationReadRepository = notificationReadRepository;
        }

        public async Task<PagedResult<GetUserAllNotificationsQueriesResponse>> Handle(GetUserAllNotificationsQueriesRequest request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationReadRepository.GetByUserIdAsync(trackChanges:false,userId:request.userId,page:request.PageNumber,pageSize:request.PageSize);
            return new PagedResult<GetUserAllNotificationsQueriesResponse>
            {
                Items = notifications.Items.Select(n => new GetUserAllNotificationsQueriesResponse
                {
                    Title = n.Title,
                    Description = n.Description,
                    SendTime = n.SendTime,
                    IsRead = n.IsRead
                }).ToList(),
                TotalCount = notifications.TotalCount,
                Page = notifications.Page,
                PageSize = notifications.PageSize
            };
        }
    }
}
