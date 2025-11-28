using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.MessageBus
{
    public class CartServiceRpcClient
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;

        public CartServiceRpcClient(IRabbitMqConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateChannel();

            _replyQueueName = _channel.QueueDeclare("", exclusive: true).QueueName;
        }

        private async Task<string> CallAsync(string queue, object message)
        {
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>();

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.TrySetResult(response);
                }
                await Task.Yield();
            };

            _channel.BasicConsume(_replyQueueName, true, consumer);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish("", queue, props, body);

            return await tcs.Task;
        }

        public async Task<CartDto?> GetCartForUser(Guid userId)
        {
            var response = await CallAsync("cart.get", new { UserId = userId });
            return JsonSerializer.Deserialize<CartDto>(response);
        }

        public async Task<bool> ClearCartForUser(Guid userId)
        {
            var response = await CallAsync("cart.clear", new { UserId = userId });
            return bool.Parse(response);
        }

        public async Task<CartDto?> RestoreItemsToCart(Guid userId, List<CartItemDto> items)
        {
            var response = await CallAsync("cart.restore", new { UserId = userId, Items = items });
            return JsonSerializer.Deserialize<CartDto>(response);
        }
    }
}
