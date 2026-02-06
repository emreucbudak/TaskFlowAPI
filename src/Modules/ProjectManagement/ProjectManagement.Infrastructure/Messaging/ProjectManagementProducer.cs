using ProjectManagement.Application.Messaging;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;

namespace ProjectManagement.Infrastructure.Messaging
{
    public class ProjectManagementProducer(IMessageQueueService messageQueueService) : IProjectManagementProducer
    {
        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            await messageQueueService.PublishMessageAsync(queueName, message);
        }
    }
}
