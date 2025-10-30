using MediatR;
using ProductService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Queries.GetProduct
{
    public class GetProductQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public GetProductQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}
