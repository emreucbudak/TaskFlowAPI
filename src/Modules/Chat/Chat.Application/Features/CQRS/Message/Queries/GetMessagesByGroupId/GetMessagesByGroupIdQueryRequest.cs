using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByGroupId
{
    public class GetMessagesByGroupIdQueryRequest : IRequest<List<GetMessagesByGroupIdQueryResponse>>
    {
        public Guid CurrentUserId { get; init; }
        public Guid GroupId { get; init; }
        public int PageSize { get; init; } = 20;
        public int Page { get; init; } = 1;

        public GetMessagesByGroupIdQueryRequest(Guid currentUserId, Guid groupId, int pageSize = 20, int page = 1)
        {
            CurrentUserId = currentUserId;
            GroupId = groupId;
            PageSize = pageSize;
            Page = page;
        }
    }
}
