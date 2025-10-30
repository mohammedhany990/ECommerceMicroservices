using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }

        public byte[]? ImageBytes { get; set; }
        public string? ImageName { get; set; }
    }
}
