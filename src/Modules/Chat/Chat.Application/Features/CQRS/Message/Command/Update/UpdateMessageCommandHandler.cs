using Chat.Application.Features.CQRS.Message.Exceptions;
using Chat.Application.Repositories;
using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Update
{
    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMessageCommandHandler(IMessageWriteRepository messageWriteRepository, IUnitOfWork unitOfWork)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var isUpdated = await _messageWriteRepository.UpdateMessageContentAsync(request.Id, request.NewContent);
            if (!isUpdated)
            {
                throw new MessageNotFoundException(request.Id);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
