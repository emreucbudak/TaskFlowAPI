using System.Threading.Tasks;

namespace Report.Application.Messaging
{
    public interface IReportProducer
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
    }
}
