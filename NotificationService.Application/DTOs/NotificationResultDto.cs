namespace NotificationService.Application.DTOs
{
    public class NotificationResultDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
