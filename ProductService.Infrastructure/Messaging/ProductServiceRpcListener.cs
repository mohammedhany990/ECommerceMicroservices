using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

            _channel.QueueDeclare(
                queue: "product.get",
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) => await HandleRequestAsync(sender, e, stoppingToken);

            _channel.BasicConsume(queue: "product.get", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task HandleRequestAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
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
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

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
                ? ApiResponse<ProductDto>.SuccessResponse(productDto, "OK", 200)
                : ApiResponse<ProductDto>.FailResponse(null!, "Product not found", 404);

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

            _channel.BasicPublish(
                exchange: "",
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes
            );

            _channel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }

    public record GetProductRequestDto(Guid ProductId);
}
