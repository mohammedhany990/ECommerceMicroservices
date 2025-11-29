using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    public class RabbitMqPublisher<T> : IRabbitMqPublisher<T>
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("notifications.create", durable: true, exclusive: false, autoDelete: false);
        }

        public void Publish<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "notifications.create",
                basicProperties: null,
                body: body);
        }
    }
}
