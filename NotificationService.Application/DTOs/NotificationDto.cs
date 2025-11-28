using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
