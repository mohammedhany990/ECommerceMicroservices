using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;
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
            if (!MailboxAddress.TryParse(request.To, out var mailbox))
            {
                _logger.LogWarning("Cannot create notification. Invalid email: {Email}", request.To);
                throw new ArgumentException("Invalid email address", nameof(request.To));
            }

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notification for UserId: {UserId}", request.UserId);
                throw;
            }
        }
    }
}
