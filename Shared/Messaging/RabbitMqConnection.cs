using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
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
