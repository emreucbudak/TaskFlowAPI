using Chat.Application.Repositories;
using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByGroupId
{
    public class GetMessagesByGroupIdQueryHandler : IRequestHandler<GetMessagesByGroupIdQueryRequest, List<GetMessagesByGroupIdQueryResponse>>
    {
        private readonly IMessageReadRepository _messageReadRepository;
        private readonly IDistributedCache _distributedCache;

        public GetMessagesByGroupIdQueryHandler(IMessageReadRepository messageReadRepository, IDistributedCache distributedCache)
        {
            _messageReadRepository = messageReadRepository;
            _distributedCache = distributedCache;
        }

        public async Task<List<GetMessagesByGroupIdQueryResponse>> Handle(GetMessagesByGroupIdQueryRequest request, CancellationToken cancellationToken)
        {
            string cacheKey = $"messages_group_{request.CurrentUserId}_{request.GroupId}_{request.PageSize}_{request.Page}";
            var cachedMessages = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedMessages))
            {
                return JsonSerializer.Deserialize<List<GetMessagesByGroupIdQueryResponse>>(cachedMessages)!;
            }

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

            var serializedResponse = JsonSerializer.Serialize(response);
            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(60)
            }, cancellationToken);

            return response;
        }
    }
}
