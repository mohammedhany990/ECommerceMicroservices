using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.MessageBus
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private readonly IConnection _connection;

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


        public IModel CreateChannel() => _connection.CreateModel();

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
