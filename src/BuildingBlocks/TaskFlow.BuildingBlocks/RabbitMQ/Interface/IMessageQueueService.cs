namespace TaskFlow.BuildingBlocks.RabbitMQ.Interface
{
    public interface IMessageQueueService
    {
        Task PublishMessageAsync<T>(string queueName, T message) where T:class;
    }
}
