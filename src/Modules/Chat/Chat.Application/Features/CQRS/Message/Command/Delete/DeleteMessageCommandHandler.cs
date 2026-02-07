using Chat.Application.Features.CQRS.Message.Exceptions;
using Chat.Application.Repositories;
using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Delete
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(IMessageWriteRepository messageWriteRepository, IUnitOfWork unitOfWork)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var isDeleted = await _messageWriteRepository.DeleteAsync(request.Id);
            if (!isDeleted)
            {
                throw new MessageNotFoundException(request.Id);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
