using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.SearchMessages
{
    public class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQueryRequest, List<SearchMessagesQueryResponse>>
    {
        public SearchMessagesQueryHandler(IMessageReadRepository messageReadRepository) { }

        public async Task<List<SearchMessagesQueryResponse>> Handle(SearchMessagesQueryRequest request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return [];
        }
    }
}
