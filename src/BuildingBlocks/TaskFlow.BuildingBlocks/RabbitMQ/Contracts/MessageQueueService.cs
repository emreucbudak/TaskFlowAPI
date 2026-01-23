using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Threading.Tasks;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;

namespace TaskFlow.BuildingBlocks.RabbitMQ.Contracts
{
    public class MessageQueueService(ILogger<MessageQueueService> logger) : IMessageQueueService,IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private ConnectionFactory? _factory;
        private async Task CreateConnection()
        {
            if (_factory is not null) return;
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "githubemre",
                Password = "emregithub63",
            };
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not create RabbitMQ connection: {ex.Message}");
            }
        }


        public Task PublishMessageAsync<T>(string queueName, T message) where T : class
        {
            
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    await _channel.CloseAsync();
                    await _channel.DisposeAsync();
                    _channel = null;
                }
                if (_connection != null && _connection.IsOpen)
                {
                    await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,$"Error disposing RabbitMQ resources: {ex.Message}");
            }
        }
    }
}
