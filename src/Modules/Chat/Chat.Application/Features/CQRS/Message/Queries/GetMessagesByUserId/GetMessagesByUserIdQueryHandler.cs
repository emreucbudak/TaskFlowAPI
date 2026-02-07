using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByUserId
{
    public class GetMessagesByUserIdQueryHandler : IRequestHandler<GetMessagesByUserIdQueryRequest, List<GetMessagesByUserIdQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;

        public GetMessagesByUserIdQueryHandler(IMessageReadRepository messageReadRepository)
        {
            _messageReadRepository = messageReadRepository;
        }

        public async Task<List<GetMessagesByUserIdQueryResponse>> Handle(GetMessagesByUserIdQueryRequest request, CancellationToken cancellationToken)
        {
            var messages = await _messageReadRepository.GetMessagesByUserIdAsync(
                request.UserId,
                request.PageSize,
                request.Page
            );

            return messages.Select(m => new GetMessagesByUserIdQueryResponse
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
