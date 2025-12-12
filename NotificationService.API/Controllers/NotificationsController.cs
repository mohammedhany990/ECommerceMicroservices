using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Models.Responses;
using NotificationService.Application.Commands.CreateNotification;
using NotificationService.Infrastructure.Messaging;
using RabbitMQ.Client;
using Shared.Messaging;
using System.Text.Json;

namespace NotificationService.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IRabbitMqConnection _rabbitMqConnection;
        private readonly UserServiceRpcClient _userServiceClient;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            IRabbitMqConnection rabbitMqConnection,
            UserServiceRpcClient userServiceClient,
            ILogger<NotificationsController> logger)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var userExists = await _userServiceClient.UserExistsAsync(command.UserId);

            if (!userExists)
            {
                _logger.LogWarning("Attempted to create notification for non-existing UserId: {UserId}", command.UserId);
                return NotFound(ApiResponse<string>.FailResponse(
                    new List<string> { $"User with Id {command.UserId} does not exist." },
                    "User not found",
                    404
                ));
            }

            using var channel = _rabbitMqConnection.CreateChannel();
            var body = JsonSerializer.SerializeToUtf8Bytes(command);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "ecommerce.events",
                routingKey: "notifications.create",
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Notification command enqueued for UserId: {UserId}", command.UserId);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Notification enqueued successfully."
            ));
        }
    }

}
