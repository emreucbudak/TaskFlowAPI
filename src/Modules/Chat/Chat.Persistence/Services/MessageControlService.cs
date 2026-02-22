using Chat.Application.ChatNotification;
using Chat.Application.Features.CQRS.Message.Command.Create;
using Chat.Application.Repositories;
using Chat.Application.Exceptions;
using TaskFlow.BuildingBlocks.UnitOfWork;
using Identity.Application.Services;

namespace Chat.Application.Services
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
                throw new MessageAuthException("Kullanıcı kimliği doğrulanmadı.");
            }

            if (senderId != currentUserId.Value)
            {
                throw new MessageAuthException("Gönderen ID, kimliği doğrulanmış kullanıcı ile eşleşmiyor.");
            }
        }

        private void ValidateMessageContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new MessageControlException("Mesaj içeriği boş olamaz.");
            }

            if (content.Length > 1000)
            {
                throw new MessageControlException("Mesaj içeriği 1000 karakter sınırını aşıyor.");
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(content, @"<[^>]+>") ||
                content.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
            {
                throw new MessageControlException("Mesaj içeriği geçersiz karakterler veya potansiyel güvenlik riskleri içeriyor.");
            }
        }

        private void ValidateMessageDestination(Guid? receiverId, Guid? groupId)
        {
            if (receiverId.HasValue && groupId.HasValue)
            {
                throw new MessageControlException("Bir mesaj hem alıcıya hem de gruba aynı anda gönderilemez.");
            }

            if (!receiverId.HasValue && !groupId.HasValue)
            {
                throw new MessageControlException("Bir mesajın ya bir alıcısı ya da bir grubu olmalıdır.");
            }
        }

        private async Task ValidateGroupMembershipAsync(Guid userId, Guid groupId)
        {
            var canSend = await _groupValidationService.ValidateGroupMembershipAsync(userId, groupId);
            if (!canSend)
            {
                throw new MessageControlException("Grup bulunamadı, pasif veya kullanıcı grubun üyesi değil.");
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
