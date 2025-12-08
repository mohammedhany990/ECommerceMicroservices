namespace CartService.Application.DTOs
{
    public class RestoreCartItemsDto
    {
        public List<CartItemDto> Items { get; set; } = new();
    }

}
