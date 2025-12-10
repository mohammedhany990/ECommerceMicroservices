using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IRepository<Product> repository,
            IMapper mapper,
            CategoryServiceRpcClient categoryServiceRpcClient,
            ILogger<GetProductByIdQueryHandler> logger
        )
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceRpcClient = categoryServiceRpcClient;
            _logger = logger;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductByIdQuery for ProductId {ProductId}", request.ProductId);

            var product = await _repository.GetByIdAsync(request.ProductId);

            if (product == null)
            {
                _logger.LogWarning("Product with Id {ProductId} not found", request.ProductId);
                return null;
            }

            var productDto = _mapper.Map<ProductDto>(product);

            var category = await _categoryServiceRpcClient.GetCategoryByIdAsync(product.CategoryId);

            if (category is null)
            {
                _logger.LogWarning("Category with Id {CategoryId} not found for ProductId {ProductId}", product.CategoryId, request.ProductId);
                productDto.CategoryName = "Unknown";
                productDto.CategoryDescription = "No description available";
            }
            else
            {
                productDto.CategoryName = category.Name;
                productDto.CategoryDescription = category.Description;
            }

            _logger.LogInformation("Product with Id {ProductId} mapped successfully with category info", request.ProductId);

            return productDto;
        }

    }
}
