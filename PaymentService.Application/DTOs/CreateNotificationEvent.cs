namespace PaymentService.Application.DTOs
{
    public class CreateNotificationEvent
    {
        public Guid UserId { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
