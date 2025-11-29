using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace CategoryService.Infrastructure.Messaging
{
    public class CategoryServiceRpcListener : BackgroundService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private IModel _channel;

        public CategoryServiceRpcListener(IRabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateChannel();

            _channel.QueueDeclare("category.getall", false, false, false);
            _channel.QueueDeclare("category.get", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) => await HandleRequestAsync(sender, e, stoppingToken);

            _channel.BasicConsume("category.getall", autoAck: false, consumer: consumer);
            _channel.BasicConsume("category.get", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task HandleRequestAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
        {
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Category>>();

            object? responseData = null;

            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());

                if (e.RoutingKey == "category.get")
                {
                    var request = JsonSerializer.Deserialize<GetCategoryRequestDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var category = await repo.GetByIdAsync(request!.CategoryId);
                    responseData = category != null ? ApiResponse<CategoryDto>.SuccessResponse(new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    }, "OK", 200)
                    : ApiResponse<CategoryDto>.FailResponse(null, "Category not found", 404);
                }
                else if (e.RoutingKey == "category.getall")
                {
                    var categories = await repo.GetAllAsync();
                    var dtos = new List<CategoryDto>();
                    foreach (var cat in categories)
                        dtos.Add(new CategoryDto { Id = cat.Id, Name = cat.Name, Description = cat.Description });

                    responseData = ApiResponse<List<CategoryDto>>.SuccessResponse(dtos, "OK", 200);
                }
            }
            catch
            {
                responseData = ApiResponse<object>.FailResponse(null, "Internal server error", 500);
            }

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseData));
            _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
            _channel.BasicAck(e.DeliveryTag, false);
        }
    }

    public record GetCategoryRequestDto(Guid CategoryId);
}
