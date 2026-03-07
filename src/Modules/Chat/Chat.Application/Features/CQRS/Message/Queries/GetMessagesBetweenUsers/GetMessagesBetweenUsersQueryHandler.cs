using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesBetweenUsers
{
    public class GetMessagesBetweenUsersQueryHandler : IRequestHandler<GetMessagesBetweenUsersQueryRequest, List<GetMessagesBetweenUsersQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;

        public GetMessagesBetweenUsersQueryHandler(IMessageReadRepository messageReadRepository)
        {
            _messageReadRepository = messageReadRepository;
        }

        public async Task<List<GetMessagesBetweenUsersQueryResponse>> Handle(GetMessagesBetweenUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var messages = await _messageReadRepository.GetMessagesBetweenUsersAsync(
                request.CurrentUserId,
                request.UserId1,
                request.UserId2,
                request.PageSize,
                request.Page
            );

            var response = messages.Select(m => new GetMessagesBetweenUsersQueryResponse
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

