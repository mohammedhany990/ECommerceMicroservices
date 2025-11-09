using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public GetProductByIdQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
