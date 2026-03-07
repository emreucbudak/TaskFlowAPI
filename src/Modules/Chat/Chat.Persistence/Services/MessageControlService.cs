using Chat.Application.ChatNotification;
using Chat.Application.Features.CQRS.Message.Command.Create;
using Chat.Application.Repositories;
using Chat.Application.Exceptions;
using Chat.Application.UnitOfWork;
using Identity.Application.Services;

namespace Chat.Application.Services
{
    public class MessageControlService : IMessageControlService
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly IChatUnitOfWork _unitOfWork;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGroupValidationService _groupValidationService;

        public MessageControlService(
            IMessageWriteRepository messageWriteRepository,
            IChatUnitOfWork unitOfWork,
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
            ValidateUserAuthentication(request.SenderId);
            ValidateMessageContent(request.Content);
            ValidateMessageDestination(request.ReceiverId, request.GroupId);

            if (request.GroupId.HasValue)
            {
                await ValidateGroupMembershipAsync(request.SenderId, request.GroupId.Value);
            }

            var message = Chat.Domain.Entities.Message.Create(
                request.Content,
                request.SenderId,
                request.ReceiverId,
                request.GroupId
            );

            await _messageWriteRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await SendNotificationsAsync(message, request.Content);
        }

        private void ValidateUserAuthentication(Guid senderId)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                throw new MessageAuthException("KullanÄ±cÄ± kimliÄŸi doÄŸrulanmadÄ±.");
            }

            if (senderId != currentUserId.Value)
            {
                throw new MessageAuthException("GÃ¶nderen ID, kimliÄŸi doÄŸrulanmÄ±ÅŸ kullanÄ±cÄ± ile eÅŸleÅŸmiyor.");
            }
        }

        private void ValidateMessageContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new MessageControlException("Mesaj iÃ§eriÄŸi boÅŸ olamaz.");
            }

            if (content.Length > 1000)
            {
                throw new MessageControlException("Mesaj iÃ§eriÄŸi 1000 karakter sÄ±nÄ±rÄ±nÄ± aÅŸÄ±yor.");
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(content, @"<[^>]+>") ||
                content.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
            {
                throw new MessageControlException("Mesaj iÃ§eriÄŸi geÃ§ersiz karakterler veya potansiyel gÃ¼venlik riskleri iÃ§eriyor.");
            }
        }

        private void ValidateMessageDestination(Guid? receiverId, Guid? groupId)
        {
            if (receiverId.HasValue && groupId.HasValue)
            {
                throw new MessageControlException("Bir mesaj hem alÄ±cÄ±ya hem de gruba aynÄ± anda gÃ¶nderilemez.");
            }

            if (!receiverId.HasValue && !groupId.HasValue)
            {
                throw new MessageControlException("Bir mesajÄ±n ya bir alÄ±cÄ±sÄ± ya da bir grubu olmalÄ±dÄ±r.");
            }
        }

        private async Task ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            var canSend = await _groupValidationService.ValidateGroupMembershipAsync(userId, groupId);
            if (!canSend)
            {
                throw new MessageControlException("Grup bulunamadÄ±, pasif veya kullanÄ±cÄ± grubun Ã¼yesi deÄŸil.");
            }
        }

        private async Task SendNotificationsAsync(Chat.Domain.Entities.Message message, string plainContent)
        {
            var notificationData = new
            {
                message.Id,
                Content = plainContent,
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

