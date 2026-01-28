namespace Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications
{
    public record GetUserAllNotificationsQueriesResponse
    {


        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime SendTime { get; init; }
        public bool IsRead { get; init; }
    }
}
