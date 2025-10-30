using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public GetProductsQuery() { }

    }
}
