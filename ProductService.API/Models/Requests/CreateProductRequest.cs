namespace ProductService.API.Models.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int QuantityInStock { get; set; }

        public int CategoryId { get; set; }

        public IFormFile? Image { get; set; }


    }
}
