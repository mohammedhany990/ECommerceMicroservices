using RabbitMQ.Client;

namespace Shared.Messaging
{
    public interface IRabbitMqConnection
    {
        IModel CreateChannel();
    }
}
