using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetUnreadMessageCount
{
    public class GetUnreadMessageCountQueryRequest : IRequest<int>
    {
        public Guid UserId { get; init; }

        public GetUnreadMessageCountQueryRequest(Guid userId)
        {
            UserId = userId;
        }
    }
}
