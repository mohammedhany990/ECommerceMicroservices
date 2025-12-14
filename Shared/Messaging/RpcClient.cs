using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests
            = new();

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
            if (e.BasicProperties.CorrelationId != null &&
                _pendingRequests.TryRemove(e.BasicProperties.CorrelationId, out var tcs))
            {
                var responseJson = Encoding.UTF8.GetString(e.Body.ToArray());
                tcs.SetResult(responseJson);
            }

            return Task.CompletedTask;
        }

        public async Task<T?> CallAsync<T>(string routingKey, object message, int timeoutMs = 15000)
        {
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            _pendingRequests[correlationId] = tcs;

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueue;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange: "", routingKey: routingKey, basicProperties: props, body: body);

            using var cts = new CancellationTokenSource(timeoutMs);
            cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            try
            {
                var json = await tcs.Task;
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (TaskCanceledException)
            {
                _pendingRequests.TryRemove(correlationId, out _);
                throw new TimeoutException("Timeout waiting for RPC response");
            }
        }
    }
}
