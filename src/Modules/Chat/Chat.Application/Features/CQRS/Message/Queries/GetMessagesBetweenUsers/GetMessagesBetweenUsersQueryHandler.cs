using Chat.Application.Repositories;
using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesBetweenUsers
{
    public class GetMessagesBetweenUsersQueryHandler : IRequestHandler<GetMessagesBetweenUsersQueryRequest, List<GetMessagesBetweenUsersQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;
        private readonly IDistributedCache _distributedCache;

        public GetMessagesBetweenUsersQueryHandler(IMessageReadRepository messageReadRepository, IDistributedCache distributedCache)
        {
            _messageReadRepository = messageReadRepository;
            _distributedCache = distributedCache;
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
                isDeleted = m.isDeleted,
                isEdited = m.isEdited,
                isDelivered = m.isDelivered,
                DeliveredTime = m.DeliveredTime
            }).ToList();
            return response;
        }
    }
}
