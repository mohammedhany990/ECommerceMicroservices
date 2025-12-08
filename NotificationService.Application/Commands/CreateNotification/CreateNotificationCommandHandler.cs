using MediatR;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly IRepository<Notification> _repository;

        public CreateNotificationCommandHandler(IRepository<Notification> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
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

            return notification.Id;
        }
    }
}
