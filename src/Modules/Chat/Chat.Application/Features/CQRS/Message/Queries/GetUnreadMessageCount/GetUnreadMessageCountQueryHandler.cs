using Chat.Application.Repositories;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Queries.GetUnreadMessageCount
{
    public class GetUnreadMessageCountQueryHandler : IRequestHandler<GetUnreadMessageCountQueryRequest, int>
    {
        private readonly IMessageReadRepository _messageReadRepository;

        public GetUnreadMessageCountQueryHandler(IMessageReadRepository messageReadRepository)
        {
            _messageReadRepository = messageReadRepository;
        }

        public async Task<int> Handle(GetUnreadMessageCountQueryRequest request, CancellationToken cancellationToken)
        {
            return await _messageReadRepository.GetUnreadMessageCountAsync(request.UserId);
        }
    }
}
