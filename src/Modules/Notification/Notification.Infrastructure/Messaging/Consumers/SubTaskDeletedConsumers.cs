using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using Notification.Application.IntegrationEvents;


namespace Notification.Infrastructure.Messaging.Consumers
{
    public class SubTaskDeletedConsumers : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SubTaskDeletedConsumers> _logger;

        public SubTaskDeletedConsumers(IMediator mediator, ILogger<SubTaskDeletedConsumers> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("SubTaskDeleted", Group = "module.notification")]
        public async Task Handle(SubTaskDeletedIntegrationEvent eventData)
        {
            var command = new CreateNotificationCommandRequest(
                title: "Alt Görev Silindi",
                description: "Üzerinizdeki bir alt görev silinmiştir.",
                sendTime: DateTime.UtcNow,
                isRead: false,
                receiverUserId: eventData.ReceiverUserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Alt görev silme bildirimi (SubTaskID: {eventData.SubTaskId}) kullanıcı {eventData.ReceiverUserId} için işlendi.");
        }
    }
}