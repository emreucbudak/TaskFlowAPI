using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskFlow.BuildingBlocks.Contracts.EventBus.Messages;
using FlashMediator;
using Notification.Application.Features.CQRS.Notification.Command.Create;

namespace Notification.Infrastructure.Messaging.Consumers
{
    public class NotifyUserConsumer : BackgroundService
    {
        private readonly ILogger<NotifyUserConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string _queueName = "notification.send";
        private readonly IMediator mediator;

        public NotifyUserConsumer(ILogger<NotifyUserConsumer> logger, IServiceProvider serviceProvider, IMediator mediator)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            InitializeRabbitMQAsync().GetAwaiter().GetResult();
            this.mediator = mediator;
        }

        private async Task InitializeRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "githubemre",
                Password = "emregithub63"
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"RabbitMQ bağlantısı oluşturulamadı: {ex.Message}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null) return;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try 
                {
                    var eventData = JsonSerializer.Deserialize<NotifyUserIntegrationEvent>(message);
                    if (eventData != null)
                    {
                        await HandleEventAsync(eventData);
                    }
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Mesaj işlenirken hata oluştu");
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        }

        private async Task HandleEventAsync(NotifyUserIntegrationEvent eventData)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                
                var command = new CreateNotificationCommandRequest(
                    "Rapor Oluşturuldu", 
                    eventData.Content, 
                    DateTime.UtcNow, 
                    false, 
                    eventData.UserId
                );

                await mediator.Send(command);
                
                _logger.LogInformation($"Kullanıcı {eventData.UserId} için bildirim kaydedildi");
            }
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
