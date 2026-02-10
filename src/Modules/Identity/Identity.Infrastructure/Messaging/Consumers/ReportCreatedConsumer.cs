using System.Text;
using System.Text.Json;
using Identity.Application.Messaging;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskFlow.BuildingBlocks.Contracts.EventBus.Messages;

namespace Identity.Infrastructure.Messaging.Consumers
{
    public class ReportCreatedConsumer : BackgroundService
    {
        private readonly ILogger<ReportCreatedConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string _queueName = "report.created";

        public ReportCreatedConsumer(ILogger<ReportCreatedConsumer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            InitializeRabbitMQAsync().GetAwaiter().GetResult();
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
                    var eventData = JsonSerializer.Deserialize<ReportCreatedIntegrationEvent>(message);
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

        private async Task HandleEventAsync(ReportCreatedIntegrationEvent eventData)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IdentityManagementDbContext>();
                var producer = scope.ServiceProvider.GetRequiredService<IIdentityProducer>();

                var leader = await dbContext.DepartmentMembers
                    .FirstOrDefaultAsync(x => x.DepartmentId == eventData.NotifiedDepartmentId && x.DepartmentRoleId == 1);

                if (leader != null)
                {
                    await producer.PublishAsync("notification.send", new NotifyUserIntegrationEvent(leader.UserId, $"Yeni Rapor: {eventData.Content}"));
                    _logger.LogInformation($"Rapor {eventData.ReportId} için lider {leader.UserId} kullanıcısına bildirim gönderildi");
                }
                else
                {
                    _logger.LogWarning($"Departman {eventData.NotifiedDepartmentId} için lider bulunamadı");
                }
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
