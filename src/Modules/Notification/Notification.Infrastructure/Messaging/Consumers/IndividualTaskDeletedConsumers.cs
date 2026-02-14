using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using Notification.Application.IntegrationEvents;


namespace Notification.Infrastructure.Messaging.Consumers
{
    public class IndividualTaskDeletedConsumers : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndividualTaskDeletedConsumers> _logger;

        public IndividualTaskDeletedConsumers(IMediator mediator, ILogger<IndividualTaskDeletedConsumers> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("IndividualTaskDeleted", Group = "module.notification")]
        public async Task Handle(IndividualTaskDeletedIntegrationEvent eventData)
        {
            var command = new CreateNotificationCommandRequest(
                title: "Görev Silindi",
                description: "Atandığınız bir bireysel görev sistemden silindi.",
                sendTime: DateTime.UtcNow,
                isRead: false,
                receiverUserId: eventData.AssignedUserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Görev silinme bildirimi (ID: {eventData.Id}) kullanıcı {eventData.AssignedUserId} için işlendi.");
        }
    }
}