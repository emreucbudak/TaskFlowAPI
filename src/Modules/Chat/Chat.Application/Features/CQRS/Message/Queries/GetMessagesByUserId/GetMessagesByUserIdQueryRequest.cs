using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByUserId
{
    public class GetMessagesByUserIdQueryRequest : IRequest<List<GetMessagesByUserIdQueryResponse>>
    {
        public Guid UserId { get; init; }
        public int PageSize { get; init; } = 20;
        public int Page { get; init; } = 1;

        public GetMessagesByUserIdQueryRequest(Guid userId, int pageSize = 20, int page = 1)
        {
            UserId = userId;
            PageSize = pageSize;
            Page = page;
        }
    }
}
