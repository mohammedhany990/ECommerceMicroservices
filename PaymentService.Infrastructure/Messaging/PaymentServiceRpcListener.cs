using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace PaymentService.Infrastructure.Messaging
{
    public class PaymentServiceRpcListener : BackgroundService
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentServiceRpcListener> _logger;
        private IModel? _channel;
        private AsyncEventingBasicConsumer? _consumer;

        private const string QueueName = "payment.getByOrder";

        public PaymentServiceRpcListener(
            IRabbitMqConnection connection,
            IServiceProvider serviceProvider,
            ILogger<PaymentServiceRpcListener> logger)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateChannel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += OnRequestReceived;

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: _consumer
            );

            _logger.LogInformation("Payment RPC listener started on queue {QueueName}", QueueName);
            return Task.CompletedTask;
        }

        private async Task OnRequestReceived(object sender, BasicDeliverEventArgs ea)
        {
            var replyProps = _channel!.CreateBasicProperties();
            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

            ApiResponse<PaymentResultDto> response;

            try
            {
                var messageJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Received RPC message: {Json}", messageJson);

                var request = JsonSerializer.Deserialize<OrderIdRequest>(messageJson);

                if (request == null)
                {
                    response = ApiResponse<PaymentResultDto>.FailResponse(
                        new List<string> { "Invalid request payload" }, "An error occurred", 400
                    );
                }
                else
                {
                    using var scope = _serviceProvider.CreateScope();
                    var paymentRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    var payment = await paymentRepo.GetByOrderIdAsync(request.OrderId);

                    if (payment == null)
                    {
                        response = ApiResponse<PaymentResultDto>.FailResponse(
                            new List<string> { "Payment not found" }, "An error occurred", 404
                        );
                    }
                    else
                    {
                        response = ApiResponse<PaymentResultDto>.SuccessResponse(
                            new PaymentResultDto
                            {
                                PaymentId = payment.Id,
                                OrderId = payment.OrderId,
                                Amount = payment.Amount,
                                Currency = payment.Currency,
                                Status = payment.Status.ToString(),
                                ConfirmedAt = payment.ConfirmedAt,
                                FailureReason = payment.FailureReason
                            },
                            "Payment retrieved successfully",
                            200
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RPC request");
                response = ApiResponse<PaymentResultDto>.FailResponse(
                    new List<string> { "Server error" }, "An error occurred", 500
                );
            }

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

            if (!string.IsNullOrEmpty(ea.BasicProperties.ReplyTo))
            {
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: ea.BasicProperties.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes
                );
            }

            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }

        public override void Dispose()
        {
            _channel?.Close();
            base.Dispose();
        }
        private class OrderIdRequest
        {
            public Guid OrderId { get; set; }
        }
    }
}
