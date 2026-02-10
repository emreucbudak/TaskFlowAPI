using Report.Application.Messaging;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using System.Threading.Tasks;

namespace Report.Infrastructure.Messaging
{
    public class ReportProducer(IMessageQueueService messageQueueService) : IReportProducer
    {
        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            await messageQueueService.PublishMessageAsync(queueName, message);
        }
    }
}
