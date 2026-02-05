namespace Identity.Application.Messaging
{
    public interface IIdentityProducer
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
    }
}
