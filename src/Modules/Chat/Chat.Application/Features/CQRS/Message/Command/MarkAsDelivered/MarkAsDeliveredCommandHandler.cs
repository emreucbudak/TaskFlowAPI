using Chat.Application.Features.CQRS.Message.Exceptions;
using Chat.Application.Repositories;
using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.MarkAsDelivered
{
    public class MarkAsDeliveredCommandHandler : IRequestHandler<MarkAsDeliveredCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAsDeliveredCommandHandler(IMessageWriteRepository messageWriteRepository, IUnitOfWork unitOfWork)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MarkAsDeliveredCommandRequest request, CancellationToken cancellationToken)
        {
            var isMarked = await _messageWriteRepository.MarkAsDeliveredAsync(request.MessageId);
            if (!isMarked)
            {
                throw new MessageNotFoundException(request.MessageId);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
