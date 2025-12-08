namespace OrderService.Application.DTOs
{
    public class CreateOrderDto
    {
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
    }
}
