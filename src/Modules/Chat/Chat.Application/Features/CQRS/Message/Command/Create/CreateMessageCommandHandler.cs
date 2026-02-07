using Chat.Application.Repositories;
using Chat.Domain.Entities;
using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Create
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMessageCommandHandler(IMessageWriteRepository messageWriteRepository, IUnitOfWork unitOfWork)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var message = new Domain.Entities.Message(
                request.Content,
                false,
                DateTime.UtcNow,
                request.SenderId,
                request.ReceiverId,
                false,
                request.GroupId,
                false
            );

            await _messageWriteRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
