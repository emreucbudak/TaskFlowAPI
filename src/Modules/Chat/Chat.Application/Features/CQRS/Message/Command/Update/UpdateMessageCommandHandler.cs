using Chat.Application.Features.CQRS.Message.Exceptions;
using Chat.Application.Repositories;
using FlashMediator;
using Chat.Application.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Update
{
    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IChatUnitOfWork _unitOfWork;

        public UpdateMessageCommandHandler(IMessageWriteRepository messageWriteRepository, IChatUnitOfWork unitOfWork)
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

