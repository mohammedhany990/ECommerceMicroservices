namespace Shared.DTOs
{
    public class CartDto
    {
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }

}
