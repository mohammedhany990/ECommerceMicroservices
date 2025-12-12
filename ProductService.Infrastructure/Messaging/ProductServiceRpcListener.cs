using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace ProductService.Infrastructure.Messaging
{
    public class ProductServiceRpcListener : BackgroundService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private IModel _channel;

        public ProductServiceRpcListener(IRabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateChannel();

            // product.get
            _channel.QueueDeclare(queue: "product.get", durable: false, exclusive: false, autoDelete: false);
            var getConsumer = new AsyncEventingBasicConsumer(_channel);
            getConsumer.Received += async (sender, e) => await HandleGetProductAsync(sender, e, stoppingToken);
            _channel.BasicConsume(queue: "product.get", autoAck: false, consumer: getConsumer);

            // product.reserve_stock
            _channel.QueueDeclare(queue: "product.reserve_stock", durable: false, exclusive: false, autoDelete: false);
            var reserveConsumer = new AsyncEventingBasicConsumer(_channel);
            reserveConsumer.Received += async (sender, e) => await HandleReserveStockAsync(sender, e, stoppingToken);
            _channel.BasicConsume(queue: "product.reserve_stock", autoAck: false, consumer: reserveConsumer);

            // product.return_stock
            _channel.QueueDeclare(queue: "product.return_stock", durable: false, exclusive: false, autoDelete: false);
            var returnConsumer = new AsyncEventingBasicConsumer(_channel);
            returnConsumer.Received += async (sender, e) => await HandleReturnStockAsync(sender, e, stoppingToken);
            _channel.BasicConsume(queue: "product.return_stock", autoAck: false, consumer: returnConsumer);

            return Task.CompletedTask;
        }

        private async Task HandleGetProductAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
        {
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

            ProductDto? productDto;
            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());
                var request = JsonSerializer.Deserialize<GetProductRequestDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request == null || request.ProductId == Guid.Empty)
                    throw new ArgumentException("Invalid ProductId");

                var product = await repo.GetByIdAsync(request.ProductId);

                productDto = product is null ? null : new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    QuantityInStock = product.QuantityInStock,
                    CategoryId = product.CategoryId,
                    ImageUrl = product.ImageUrl,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };
            }
            catch
            {
                productDto = null;
            }

            var response = productDto is not null
                ? ApiResponse<ProductDto>.SuccessResponse(productDto, "Product retrieved successfully", 200)
                : ApiResponse<ProductDto>.FailResponse(null!, "Product not found", 404);

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }

        private async Task HandleReserveStockAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
        {
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            bool success = false;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());
                var request = JsonSerializer.Deserialize<ReserveStockRequestDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request != null && request.ProductId != Guid.Empty && request.Quantity > 0)
                {
                    success = await repo.TryReserveStockAsync(request.ProductId, request.Quantity);
                }
            }
            catch
            {
                success = false;
            }

            var response = new { Success = success };
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }

        private async Task HandleReturnStockAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
        {
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            bool success = false;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());
                var request = JsonSerializer.Deserialize<ReturnStockRequestDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request != null && request.ProductId != Guid.Empty && request.Quantity > 0)
                {
                    success = await repo.ReturnStockAsync(request.ProductId, request.Quantity);
                }
            }
            catch
            {
                success = false;
            }

            var response = new { Success = success };
            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
            _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }

    public record GetProductRequestDto(Guid ProductId);
    public record ReserveStockRequestDto(Guid ProductId, int Quantity);
    public record ReturnStockRequestDto(Guid ProductId, int Quantity);
}
