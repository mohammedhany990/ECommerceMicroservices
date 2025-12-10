using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly IRepository<Notification> _repository;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            IRepository<Notification> repository,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating notification for UserId: {UserId}, To: {To}", request.UserId, request.To);

            var notification = new Notification
            {
                UserId = request.UserId,
                To = request.To,
                Subject = request.Subject,
                Body = request.Body,
                Type = request.Type
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Notification created successfully with Id: {NotificationId}", notification.Id);

            return notification.Id;
        }
    }
}