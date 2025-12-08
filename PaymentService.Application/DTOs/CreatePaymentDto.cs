namespace PaymentService.Application.DTOs
{
    public class CreatePaymentDto
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
    }
}
