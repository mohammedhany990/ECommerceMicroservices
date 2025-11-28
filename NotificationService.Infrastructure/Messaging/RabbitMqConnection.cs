using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Messaging
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private readonly IConnection _connection;
        private IModel? _channel;

        public RabbitMqConnection(IConfiguration config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"],
                UserName = config["RabbitMQ:User"],
                Password = config["RabbitMQ:Pass"],
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
        }
        public IModel CreateChannel()
        {
            if (_channel != null && _channel.IsOpen) return _channel;

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("ecommerce.events", ExchangeType.Topic, durable: true);
            _channel.QueueDeclare("notifications.create", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("notifications.create", "ecommerce.events", "#");
            return _channel;
        }

        public void Dispose() => _connection?.Dispose();
    }

}
