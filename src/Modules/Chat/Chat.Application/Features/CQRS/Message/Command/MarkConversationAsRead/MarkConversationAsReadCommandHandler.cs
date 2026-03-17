using Chat.Application.Repositories;
using Chat.Application.UnitOfWork;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.MarkConversationAsRead
{
    public sealed class MarkConversationAsReadCommandHandler(
        IMessageWriteRepository messageWriteRepository,
        IChatUnitOfWork unitOfWork) : IRequestHandler<MarkConversationAsReadCommandRequest, int>
    {
        public async Task<int> Handle(MarkConversationAsReadCommandRequest request, CancellationToken cancellationToken)
        {
            var markedCount = await messageWriteRepository.MarkConversationAsReadAsync(request.CurrentUserId, request.OtherUserId);
            if (markedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return markedCount;
        }
    }
}
