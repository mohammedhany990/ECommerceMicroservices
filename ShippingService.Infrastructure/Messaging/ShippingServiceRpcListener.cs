using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using ShippingService.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace ShippingService.Infrastructure.Messaging
{
    public class ShippingServiceRpcListener : IHostedService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private IModel? _channel;

        public ShippingServiceRpcListener(IRabbitMqConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateChannel();

            // Declare queues
            _channel.QueueDeclare("shipping.calculate", false, false, false, null);
            _channel.QueueDeclare("shipping.get", false, false, false, null);

            // Start consumers
            StartConsumer<ShippingCostRequestDto, ShippingCostResultDto, IShippingCostCalculator>(
                "shipping.calculate",
                (service, request) => service.CalculateAsync(request.ShippingAddressId, request.ShippingMethodId));

            StartConsumer<IdWrapper, ShippingMethod, IRepository<ShippingMethod>>(
                "shipping.get",
                (service, request) => service.GetByIdAsync(request.Id));

            return Task.CompletedTask;
        }

        private void StartConsumer<TRequest, TResponse, TService>(
            string queueName,
            Func<TService, TRequest, Task<TResponse>> handle)
            where TService : notnull
        {
            if (_channel == null) throw new InvalidOperationException("Channel not initialized");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();

                var request = JsonSerializer.Deserialize<TRequest>(ea.Body.ToArray());
                if (request == null)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                var result = await handle(service, request);

                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result));
                var props = _channel.CreateBasicProperties();
                props.CorrelationId = ea.BasicProperties.CorrelationId;

                _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: props, body: responseBytes);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queueName, autoAck: false, consumer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            return Task.CompletedTask;
        }

        private class IdWrapper
        {
            public Guid Id { get; set; }
        }
    }
}
