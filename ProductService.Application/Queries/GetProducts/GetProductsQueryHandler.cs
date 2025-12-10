using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;

namespace ProductService.Application.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        private readonly ILogger<GetProductsQueryHandler> _logger;

        public GetProductsQueryHandler(
            IRepository<Product> repository,
            IMapper mapper,
            CategoryServiceRpcClient categoryServiceRpcClient,
            ILogger<GetProductsQueryHandler> logger
        )
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceRpcClient = categoryServiceRpcClient;
            _logger = logger;
        }


        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductsQuery");

            var products = await _repository.GetAllAsync();

            if (!products.Any())
            {
                _logger.LogInformation("No products found");
                return new List<ProductDto>();
            }

            _logger.LogInformation("Retrieved {Count} products from repository", products.Count().ToString());

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            var categories = await _categoryServiceRpcClient.GetAllCategoriesAsync();
            _logger.LogInformation("Retrieved {Count} categories from CategoryService", categories.Count);

            var categoryLookup = categories.ToDictionary(c => c.Id, c => c);

            foreach (var productDto in productDtos)
            {
                if (categoryLookup.TryGetValue(productDto.CategoryId, out var category))
                {
                    productDto.CategoryName = category.Name;
                    productDto.CategoryDescription = category.Description;
                }
                else
                {
                    productDto.CategoryName = "Unknown";
                    productDto.CategoryDescription = "No description available";
                    _logger.LogWarning("Category for ProductId {ProductId} not found", productDto.Id);
                }
            }

            _logger.LogInformation("Mapped products with category information successfully");

            return productDtos;
        }

    }
}
