using FlashMediator;
using Notification.Application.Repositories;

namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public class GetUserAllNotificationsQueriesHandler : IRequestHandler<GetUserAllNotificationsQueriesRequest, List<GetUserAllNotificationsQueriesResponse>>
    {
        private readonly INotificationReadRepository _notificationReadRepository;

        public GetUserAllNotificationsQueriesHandler(INotificationReadRepository notificationReadRepository)
        {
            _notificationReadRepository = notificationReadRepository;
        }

        public async Task<List<GetUserAllNotificationsQueriesResponse>> Handle(GetUserAllNotificationsQueriesRequest request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationReadRepository.GetByUserIdAsync(trackChanges:false,userId:request.userId,take:request.Take);
            return notifications.Select(n => new GetUserAllNotificationsQueriesResponse
            {
                Title = n.Title,
                Description = n.Description,
                SendTime = n.SendTime,
                IsRead = n.IsRead
            }).ToList();
        }
    }
}
