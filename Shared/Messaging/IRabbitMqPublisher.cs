namespace Shared.Messaging
{
    public interface IRabbitMqPublisher<T>
    {
        void Publish<T>(T message);
    }
}
