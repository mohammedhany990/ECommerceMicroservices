using Shared.Enums;

namespace OrderService.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public string? ShippingMethod { get; set; } = string.Empty;
        public DateTime? ExpectedDeliveryDate { get; set; }

        public string PaymentStatus { get; set; } 
        public Guid? PaymentId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }


}
