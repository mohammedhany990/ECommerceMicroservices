using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
