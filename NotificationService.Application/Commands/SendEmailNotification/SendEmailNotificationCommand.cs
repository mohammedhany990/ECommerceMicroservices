using MediatR;
using NotificationService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Commands.SendEmailNotification
{
    public class SendEmailNotificationCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
    
}
