using MediatR;
using NotificationService.Application.Commands.CreateNotification;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging;
using System.Text.Json;

public class RabbitMqListener : BackgroundService
{
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    private const string QueueName = "notifications.create";
    private const string ExchangeName = "ecommerce.events";

    public RabbitMqListener(IRabbitMqConnection connection, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _channel = connection.CreateChannel();
        _channel.QueueBind(QueueName, ExchangeName, "#");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            Console.WriteLine("Message received!");

            var command = JsonSerializer.Deserialize<CreateNotificationCommand>(ea.Body.ToArray());

            if (command != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(command, stoppingToken);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}
