using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesBetweenUsers
{
    public class GetMessagesBetweenUsersQueryRequest : IRequest<List<GetMessagesBetweenUsersQueryResponse>>, ICacheableQuery
    {
        public Guid CurrentUserId { get; init; }
        public Guid UserId1 { get; init; }
        public Guid UserId2 { get; init; }
        public int PageSize { get; init; } = 20;
        public int Page { get; init; } = 1;

        public string CacheKey => UserId1.ToString()+UserId2.ToString();

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(60);

        public GetMessagesBetweenUsersQueryRequest(Guid currentUserId, Guid userId1, Guid userId2, int pageSize = 20, int page = 1)
        {
            CurrentUserId = currentUserId;
            UserId1 = userId1;
            UserId2 = userId2;
            PageSize = pageSize;
            Page = page;
        }
    }
}
