using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;

namespace ProductService.Application.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        public GetProductByIdQueryHandler(
            IRepository<Product> repository,
            IMapper mapper,
            CategoryServiceRpcClient categoryServiceRpcClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceRpcClient = categoryServiceRpcClient;
        }
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return null;
            }
            var productDto = _mapper.Map<ProductDto>(product);

            var category = await _categoryServiceRpcClient.GetCategoryByIdAsync(product.CategoryId);

            productDto.CategoryName = category?.Name ?? "Unknown";
            productDto.CategoryDescription = category?.Description ?? "No description available";

            return productDto;
        }
    }
}
