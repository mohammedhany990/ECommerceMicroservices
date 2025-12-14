using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Enums;
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

        public OrderServiceRpcListener(
            IRabbitMqConnection connection,
            IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateChannel();

            _channel.QueueDeclare("order.request", false, false, false);
            _channel.QueueDeclare("order.updateStatus", false, false, false);
            _channel.QueueDeclare("order.updatePaymentStatus", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;

            _channel.BasicConsume("order.request", false, consumer);
            _channel.BasicConsume("order.updateStatus", false, consumer);
            _channel.BasicConsume("order.updatePaymentStatus", false, consumer);

            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();

            try
            {
                switch (ea.RoutingKey)
                {
                    case "order.request":
                        await HandleGetOrder(ea, repository);
                        break;

                    case "order.updateStatus":
                        await HandleUpdateOrderStatus(ea, repository);
                        break;

                    case "order.updatePaymentStatus":
                        await HandleUpdatePaymentStatus(ea, repository);
                        break;
                }
            }
            finally
            {
                _channel!.BasicAck(ea.DeliveryTag, false);
            }
        }

        //GET ORDER
        private async Task HandleGetOrder(BasicDeliverEventArgs ea, IRepository<Order> repository)
        {
            var orderId = JsonSerializer.Deserialize<Guid>(ea.Body.ToArray());
            var order = await repository.GetByIdAsync(orderId);

            var response = order == null ? null : new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                ShippingAddressId = order.ShippingAddressId,
                ShippingMethodId = order.ShippingMethodId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                ShippingCost = order.ShippingCost,
                TotalAmount = order.TotalAmount,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                PaymentStatus = order.PaymentStatus.ToString(),
                PaymentId = order.PaymentId,
                Items = order.Items?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    ImageUrl = i.ImageUrl
                }).ToList() ?? new()
            };

            PublishResponse(ea, response);
        }

        // UPDATE ORDER STATUS
        private async Task HandleUpdateOrderStatus(BasicDeliverEventArgs ea, IRepository<Order> repository)
        {
            var payload = JsonSerializer.Deserialize<UpdateOrderStatusRpcRequest>(ea.Body.ToArray());
            if (payload == null)
            {
                PublishResponse(ea, false);
                return;
            }

            var order = await repository.GetByIdAsync(payload.OrderId);
            bool success = false;

            if (order != null && Enum.TryParse<OrderStatus>(payload.Status, out var status))
            {
                order.Status = status;
                await repository.SaveChangesAsync();
                success = true;
            }

            PublishResponse(ea, success);
        }

        // UPDATE PAYMENT STATUS
        private async Task HandleUpdatePaymentStatus(BasicDeliverEventArgs ea, IRepository<Order> repository)
        {
            var payload = JsonSerializer.Deserialize<UpdateOrderPaymentStatusRpcRequest>(ea.Body.ToArray());
            if (payload == null)
            {
                PublishResponse(ea, false);
                return;
            }

            var order = await repository.GetByIdAsync(payload.OrderId);
            bool success = false;

            if (order != null && Enum.TryParse<PaymentStatus>(payload.PaymentStatus, out var paymentStatus))
            {
                order.PaymentStatus = paymentStatus;
                await repository.SaveChangesAsync();
                success = true;
            }

            PublishResponse(ea, success);
        }


        private void PublishResponse(BasicDeliverEventArgs ea, object response)
        {
            var replyProps = _channel!.CreateBasicProperties();
            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

            _channel.BasicPublish(
                exchange: "",
                routingKey: ea.BasicProperties.ReplyTo,
                basicProperties: replyProps,
                body: body);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            return Task.CompletedTask;
        }
    }

    public class UpdateOrderStatusRpcRequest
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateOrderPaymentStatusRpcRequest
    {
        public Guid OrderId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
