using Shared.Enums;

namespace Shared.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public string? ShippingMethod { get; set; } = string.Empty;
        public DateTime? ExpectedDeliveryDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public Guid? PaymentId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

}
