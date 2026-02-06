namespace ProjectManagement.Application.Messaging
{
    public interface IProjectManagementProducer
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
    }
}
