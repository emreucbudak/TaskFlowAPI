using Chat.Application.ChatNotification;
using Chat.Application.Repositories;
using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Application.Features.CQRS.Message.Command.Create
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest>
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatNotificationService _chatNotificationService;

        public CreateMessageCommandHandler(IMessageWriteRepository messageWriteRepository, IUnitOfWork unitOfWork, IChatNotificationService chatNotificationService)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
            _chatNotificationService = chatNotificationService;
        }

        public async Task Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var message = Domain.Entities.Message.Create(
                request.Content,
                request.SenderId,
                request.ReceiverId,
                request.GroupId
            );

            await _messageWriteRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var notificationData = new
            {
                message.Id,
                message.Content,
                message.SenderId,
                message.SendTime,
                message.ReceiverId,
                message.GroupId
            };

            if (message.GroupId.HasValue)
            {
                await _chatNotificationService.SendMessageToGroupAsync(message.GroupId.Value, notificationData);
            }
            else if (message.ReceiverId.HasValue)
            {
                await _chatNotificationService.SendMessageToUserAsync(message.ReceiverId.Value, notificationData);
            }
        }
    }
}