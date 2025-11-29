using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Messaging
{
    public class OrderServiceRpcClient
    {
        private readonly IRabbitMqConnection _connection;

        public OrderServiceRpcClient(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            using var channel = _connection.CreateChannel();

            var replyQueue = channel.QueueDeclare().QueueName;
            var correlationId = Guid.NewGuid().ToString();

            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueue;

            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderId));
            channel.BasicPublish(exchange: "", routingKey: "order.request", basicProperties: props, body: messageBytes);

            var tcs = new TaskCompletionSource<OrderDto?>();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = ea.Body.ToArray();
                    var order = JsonSerializer.Deserialize<OrderDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    tcs.SetResult(order);
                }
            };

            channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);

            using (cancellationToken.Register(() => tcs.SetCanceled()))
            {
                return await tcs.Task;
            }
        }
    }
}
