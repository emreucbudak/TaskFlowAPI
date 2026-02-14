using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using Notification.Application.IntegrationEvents;


namespace Notification.Infrastructure.Messaging.Consumers
{
    public class IndividualTaskUpdatedConsumers : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndividualTaskUpdatedConsumers> _logger;

        public IndividualTaskUpdatedConsumers(IMediator mediator, ILogger<IndividualTaskUpdatedConsumers> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("IndividualTaskUpdated", Group = "module.notification")]
        public async Task Handle(IndividualTaskUpdatedIntegrationEvent eventData)
        {
            var content = $"'{eventData.TaskTitle}' isimli göreviniz güncellendi.";

            var command = new CreateNotificationCommandRequest(
                title: "Görev Güncellendi",
                description: content,
                sendTime: DateTime.UtcNow,
                isRead: false,
                receiverUserId: eventData.AssignedUserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Görev güncelleme bildirimi (ID: {eventData.Id}) kullanıcı {eventData.AssignedUserId} için Mediator'a iletildi.");
        }
    }
}