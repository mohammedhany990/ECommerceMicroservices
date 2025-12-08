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

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) => await HandleRequestAsync(sender, e, stoppingToken);

            _channel.BasicConsume("user.getemail", autoAck: false, consumer: consumer);
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
                var request = JsonSerializer.Deserialize<GetUserEmailRequestDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request == null || request.UserId == Guid.Empty)
                    throw new ArgumentException("Invalid UserId");

                var email = await userRepo.GetUserEmailAsync(request.UserId);

                responseData = ApiResponse<string>.SuccessResponse(email, "OK", 200);
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
}
