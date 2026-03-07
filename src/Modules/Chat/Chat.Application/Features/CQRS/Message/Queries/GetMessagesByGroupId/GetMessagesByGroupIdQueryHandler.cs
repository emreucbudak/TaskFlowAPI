using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByGroupId
{
    public class GetMessagesByGroupIdQueryHandler : IRequestHandler<GetMessagesByGroupIdQueryRequest, List<GetMessagesByGroupIdQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;

        public GetMessagesByGroupIdQueryHandler(IMessageReadRepository messageReadRepository)
        {
            _messageReadRepository = messageReadRepository;
        }

        public async Task<List<GetMessagesByGroupIdQueryResponse>> Handle(GetMessagesByGroupIdQueryRequest request, CancellationToken cancellationToken)
        {

            var messages = await _messageReadRepository.GetMessagesByGroupIdAsync(
                request.CurrentUserId,
                request.GroupId,
                request.PageSize,
                request.Page
            );

            var response = messages.Select(m => new GetMessagesByGroupIdQueryResponse
            {
                Id = m.Id,
                Content = m.Content,
                IsRead = m.IsRead,
                SendTime = m.SendTime,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                GroupId = m.GroupId,
                IsDeleted = m.IsDeleted,
                IsEdited = m.IsEdited,
                IsDelivered = m.IsDelivered,
                DeliveredTime = m.DeliveredTime
            }).ToList();


            return response;
        }
    }
}

