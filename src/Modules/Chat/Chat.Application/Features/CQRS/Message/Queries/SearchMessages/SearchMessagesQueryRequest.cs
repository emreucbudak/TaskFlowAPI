using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.SearchMessages
{
    public class SearchMessagesQueryRequest : IRequest<List<SearchMessagesQueryResponse>>
    {
        public Guid CurrentUserId { get; init; }
        public string SearchTerm { get; init; }
        public int PageSize { get; init; } = 20;
        public int Page { get; init; } = 1;

        public SearchMessagesQueryRequest(Guid currentUserId, string searchTerm, int pageSize = 20, int page = 1)
        {
            CurrentUserId = currentUserId;
            SearchTerm = searchTerm;
            PageSize = pageSize;
            Page = page;
        }
    }
}
