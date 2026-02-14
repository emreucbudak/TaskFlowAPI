using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;


namespace Notification.Infrastructure.Messaging.Consumers
{
    public class SubTaskUpdatedConsumers : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SubTaskUpdatedConsumers> _logger;

        public SubTaskUpdatedConsumers(IMediator mediator, ILogger<SubTaskUpdatedConsumers> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("SubTaskUpdated", Group = "module.notification")]
        public async Task Handle(SubTaskUpdatedIntegrationEvent eventData)
        {
            var content = $"'{eventData.TaskTitle}' isimli alt göreviniz güncellendi.";

            var command = new CreateNotificationCommandRequest(
                title: "Alt Görev Güncellendi",
                description: content,
                sendTime: DateTime.UtcNow,
                isRead: false,
                receiverUserId: eventData.ReceiverUserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Alt görev güncelleme bildirimi (ID: {eventData.SubTaskId}) kullanıcı {eventData.ReceiverUserId} için işlendi.");
        }
    }
}