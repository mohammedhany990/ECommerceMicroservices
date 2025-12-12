using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System.Text;
using System.Text.Json;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Messaging
{
    public class UserServiceRpcListener : BackgroundService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private IModel _channel;

        public UserServiceRpcListener(IRabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateChannel();

            _channel.QueueDeclare("user.getemail", false, false, false);
            _channel.QueueDeclare("user.exists", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) => await HandleRequestAsync(sender, e, stoppingToken);

            _channel.BasicConsume("user.getemail", autoAck: false, consumer: consumer);
            _channel.BasicConsume("user.exists", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task HandleRequestAsync(object sender, BasicDeliverEventArgs e, CancellationToken cancellationToken)
        {
            var props = e.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            object responseData;

            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());

                if (e.RoutingKey == "user.getemail")
                {
                    var request = JsonSerializer.Deserialize<GetUserEmailRequestDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (request == null || request.UserId == Guid.Empty)
                        throw new ArgumentException("Invalid UserId");

                    var email = await userRepo.GetUserEmailAsync(request.UserId);
                    responseData = ApiResponse<string>.SuccessResponse(email, "OK", 200);
                }
                else if (e.RoutingKey == "user.exists")
                {
                    var request = JsonSerializer.Deserialize<UserExistsRequestDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (request == null || request.UserId == Guid.Empty)
                        throw new ArgumentException("Invalid UserId");

                    var exists = await userRepo.ExistsAsync(u => u.Id == request.UserId);
                    responseData = ApiResponse<bool>.SuccessResponse(exists, "OK", 200);
                }
                else
                {
                    responseData = ApiResponse<string>.FailResponse(null, "Unknown routing key", 400);
                }
            }
            catch
            {
                responseData = ApiResponse<string>.FailResponse(null, "Internal server error", 500);
            }

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseData));
            _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
            _channel.BasicAck(e.DeliveryTag, false);
        }
    }

    public record GetUserEmailRequestDto(Guid UserId);
    public record UserExistsRequestDto(Guid UserId);
}
