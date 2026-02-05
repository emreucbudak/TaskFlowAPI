using Identity.Application.Messaging;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;

namespace Identity.Infrastructure.Messaging
{
    public class IdentityProducer(IMessageQueueService messageQueueService) : IIdentityProducer
    {
        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            await messageQueueService.PublishMessageAsync(queueName, message);
        }
    }
}
