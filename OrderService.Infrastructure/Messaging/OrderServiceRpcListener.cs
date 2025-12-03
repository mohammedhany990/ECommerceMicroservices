using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging
{
    public class OrderServiceRpcListener : IHostedService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private IModel? _channel;

        public OrderServiceRpcListener(IRabbitMqConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateChannel();
            _channel.QueueDeclare(
                queue: "order.request",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();

                var orderId = JsonSerializer.Deserialize<Guid>(ea.Body.ToArray());
                var order = await repository.GetByIdAsync(orderId);

                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));

                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: ea.BasicProperties.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);

            };

            _channel.BasicConsume(
                queue: "order.request",
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            return Task.CompletedTask;
        }
    }
}
