using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace CartService.InfraStructure.Messaging
{
    public class CartServiceRpcListener : BackgroundService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        public CartServiceRpcListener(IRabbitMqConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queues = new[] { "cart.get", "cart.clear", "cart.restore" };

            foreach (var queue in queues)
            {
                var channel = _connection.CreateChannel();
                channel.QueueDeclare(queue, false, false, false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (_, ea) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ICartRepository>();

                    try
                    {
                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var request = JsonSerializer.Deserialize<RpcCartRequest>(json)!;

                        object? response = queue switch
                        {
                            "cart.get" =>
                                await repo.GetCartAsync(request.UserId),

                            "cart.clear" =>
                                await repo.ClearCartAsync(request.UserId),

                            "cart.restore" =>
                                await repo.RestoreItemsAsync(request.UserId, request.Items ?? new()),

                            _ => null
                        };

                        var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                        var props = channel.CreateBasicProperties();
                        props.CorrelationId = ea.BasicProperties.CorrelationId;

                        channel.BasicPublish("", ea.BasicProperties.ReplyTo, props, responseBytes);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[RPC Listener Error] {ex.Message}");
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };

                channel.BasicConsume(queue, autoAck: false, consumer);
            }

            return Task.CompletedTask;
        }
    }

    public class RpcCartRequest
    {
        public Guid UserId { get; set; }
        public List<CartItem>? Items { get; set; }
    }
}
