using Chat.Application.ChatNotification;
using Chat.Application.Features.CQRS.Message.Command.Create;
using Chat.Application.Repositories;
using Chat.Application.Services;
using TaskFlow.BuildingBlocks.Bases.Exceptions;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Chat.Infrastructure.Services
{
    public class MessageControlService : IMessageControlService
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGroupValidationService _groupValidationService;

        public MessageControlService(
            IMessageWriteRepository messageWriteRepository,
            IUnitOfWork unitOfWork,
            IChatNotificationService chatNotificationService,
            ICurrentUserService currentUserService,
            IGroupValidationService groupValidationService)
        {
            _messageWriteRepository = messageWriteRepository;
            _unitOfWork = unitOfWork;
            _chatNotificationService = chatNotificationService;
            _currentUserService = currentUserService;
            _groupValidationService = groupValidationService;
        }

        public async Task HandleMessageCreationAsync(CreateMessageCommandRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                throw new AuthExceptions("User is not authenticated.");
            }

            if (request.SenderId != currentUserId.Value)
            {
                throw new AuthExceptions("SenderId does not match the authenticated user.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new BaseExceptions("Message content cannot be empty.");
            }

            if (request.Content.Length > 1000)
            {
                throw new BaseExceptions("Message content exceeds the maximum length of 1000 characters.");
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(request.Content, @"<[^>]+>") ||
                request.Content.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
            {
                throw new BaseExceptions("Message content contains invalid characters or potential security risks.");
            }

            if (request.ReceiverId.HasValue && request.GroupId.HasValue)
            {
                throw new BaseExceptions("A message cannot have both a ReceiverId and a GroupId.");
            }

            if (!request.ReceiverId.HasValue && !request.GroupId.HasValue)
            {
                throw new BaseExceptions("A message must have either a ReceiverId or a GroupId.");
            }

            if (request.GroupId.HasValue)
            {
                var canSend = await _groupValidationService.ValidateGroupMembershipAsync(currentUserId.Value, request.GroupId.Value);
                if (!canSend)
                {
                    throw new BaseExceptions("Group not found, inactive, or user is not a member.");
                }
            }

            var message = Chat.Domain.Entities.Message.Create(
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
