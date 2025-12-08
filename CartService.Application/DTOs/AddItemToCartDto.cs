namespace CartService.Application.DTOs
{
    public class AddItemToCartDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
