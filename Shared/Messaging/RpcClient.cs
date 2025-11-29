using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Messaging
{
    public class RpcClient
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IModel _channel;

        private readonly string _replyQueue;
        private readonly AsyncEventingBasicConsumer _consumer;

        private string? _correlationId;
        private TaskCompletionSource<string>? _tcs;

        public RpcClient(IRabbitMqConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateChannel();

            _replyQueue = _channel.QueueDeclare("", exclusive: true).QueueName;

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += OnResponseReceived;

            _channel.BasicConsume(_replyQueue, autoAck: true, _consumer);
        }

        private Task OnResponseReceived(object sender, BasicDeliverEventArgs e)
        {
            if (e.BasicProperties.CorrelationId == _correlationId)
            {
                var responseJson = Encoding.UTF8.GetString(e.Body.ToArray());
                _tcs?.SetResult(responseJson);
            }

            return Task.CompletedTask;
        }

        public async Task<T?> Call<T>(string routingKey, object message)
        {
            _correlationId = Guid.NewGuid().ToString();
            _tcs = new TaskCompletionSource<string>();

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = _correlationId;
            props.ReplyTo = _replyQueue;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _channel.BasicPublish(
                exchange: "",
                routingKey: routingKey,
                basicProperties: props,
                body: body
            );

            var json = await _tcs.Task;

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
