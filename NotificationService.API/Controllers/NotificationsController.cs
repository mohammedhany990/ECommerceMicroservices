using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands.CreateNotification;
using RabbitMQ.Client;
using Shared.Messaging;
using System.Text.Json;

namespace NotificationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IModel _channel;
        private readonly IRabbitMqConnection _rabbitMqConnection;

        public NotificationsController(IRabbitMqConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var channel = _rabbitMqConnection.CreateChannel();
            var body = JsonSerializer.SerializeToUtf8Bytes(command);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "ecommerce.events",
                routingKey: "notifications.create",
                basicProperties: properties,
                body: body
            );

            return Ok();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateNotificationCommand command)
        //{
        //    var id = await _mediator.Send(command);
        //    return Ok(new { notificationId = id });
        //}
    }
}
