using MediatR;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Commands.SendEmailNotification
{
    public class SendEmailNotificationCommandHandler : IRequestHandler<SendEmailNotificationCommand, Guid>
    {
        private readonly IRepository<Notification> _repository;

        public SendEmailNotificationCommandHandler(IRepository<Notification> repository)
        {
            _repository = repository;
        }

        public Task<Guid> Handle(SendEmailNotificationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
