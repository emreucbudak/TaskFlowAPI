using Chat.Application.Features.CQRS.Message.Exceptions;
using Chat.Application.Repositories;
using FlashMediator;
using Chat.Application.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Delete
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommandRequest>
    {
        private readonly IMessageReadRepository _messageReadRepository;
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IChatUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(
            IMessageReadRepository messageReadRepository,
            IMessageWriteRepository messageWriteRepository,
            IChatUnitOfWork unitOfWork)
        {
            _messageReadRepository = messageReadRepository;
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var message = await _messageReadRepository.GetByIdAsync(trackChanges: false, request.Id);
            if (message is null || message.Id == Guid.Empty)
            {
                throw new MessageNotFoundException(request.Id);
            }

            if (message.SenderId != request.CurrentUserId)
            {
                throw new MessageOwnershipException(request.Id);
            }

            var isDeleted = await _messageWriteRepository.DeleteAsync(request.Id);
            if (!isDeleted)
            {
                throw new MessageNotFoundException(request.Id);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
