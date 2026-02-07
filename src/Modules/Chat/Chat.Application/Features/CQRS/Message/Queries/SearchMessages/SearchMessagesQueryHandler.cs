using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.SearchMessages
{
    public class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQueryRequest, List<SearchMessagesQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;

        public SearchMessagesQueryHandler(IMessageReadRepository messageReadRepository)
        {
            _messageReadRepository = messageReadRepository;
        }

        public async Task<List<SearchMessagesQueryResponse>> Handle(SearchMessagesQueryRequest request, CancellationToken cancellationToken)
        {
            var messages = await _messageReadRepository.SearchMessagesAsync(
                request.CurrentUserId,
                request.SearchTerm,
                request.PageSize,
                request.Page
            );

            return messages.Select(m => new SearchMessagesQueryResponse
            {
                Id = m.Id,
                Content = m.Content,
                IsRead = m.IsRead,
                SendTime = m.SendTime,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                GroupId = m.GroupId,
                isDeleted = m.isDeleted,
                isEdited = m.isEdited,
                isDelivered = m.isDelivered,
                DeliveredTime = m.DeliveredTime
            }).ToList();
        }
    }
}
