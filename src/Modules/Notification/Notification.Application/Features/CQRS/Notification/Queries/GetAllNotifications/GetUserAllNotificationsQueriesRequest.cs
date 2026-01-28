using FlashMediator;

namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public record GetUserAllNotificationsQueriesRequest : IRequest<List<GetUserAllNotificationsQueriesResponse>>
    {
        public GetUserAllNotificationsQueriesRequest(Guid userId, int take)
        {
            this.userId = userId;
            Take = take;
        }

        public Guid userId { get; init; }
        public int Take { get; init; }
        }
}
