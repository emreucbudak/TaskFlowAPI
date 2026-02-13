using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using TaskFlow.BuildingBlocks.Contracts.EventBus.Messages;

namespace Notification.Infrastructure.Messaging.Consumers
{
    public class NotifyUserConsumer : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotifyUserConsumer> _logger;

        public NotifyUserConsumer(IMediator mediator, ILogger<NotifyUserConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe("notification.send", Group = "module.notification")]
        public async Task ConsumeAsync(NotifyUserIntegrationEvent eventData)
        {
            var command = new CreateNotificationCommandRequest(
                "Rapor Oluşturuldu",
                eventData.Content,
                DateTime.UtcNow,
                false,
                eventData.UserId
            );

            await _mediator.Send(command);

            _logger.LogInformation($"Kullanıcı {eventData.UserId} için bildirim başarıyla oluşturuldu.");
        }
    }
}