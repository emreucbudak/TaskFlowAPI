using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using Notification.Application.IntegrationEvents;


namespace Notification.Infrastructure.Messaging.Consumers
{
    public class SubTaskCreatedConsumers : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SubTaskCreatedConsumers> _logger;

        public SubTaskCreatedConsumers(IMediator mediator, ILogger<SubTaskCreatedConsumers> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("SubTaskCreated", Group = "module.notification")]
        public async Task Handle(SubTaskCreatedIntegrationEvent eventData)
        {
            var content = $"Yeni bir alt görev atandı: {eventData.TaskTitle}.";

            var command = new CreateNotificationCommandRequest(
                title: "Yeni Alt Görev",
                description: content,
                sendTime: DateTime.UtcNow,
                isRead: false,
                receiverUserId: eventData.AssignedUserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Alt görev bildirimi (TaskID: {eventData.TaskId}) kullanıcı {eventData.AssignedUserId} için Mediator'a gönderildi.");
        }
    }
}