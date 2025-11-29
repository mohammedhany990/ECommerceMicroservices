using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;
using ProductService.Infrastructure.Services;

namespace ProductService.Application.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        public GetProductsQueryHandler(
            IRepository<Product> repository,
            IMapper mapper,
            CategoryServiceRpcClient categoryServiceRpcClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceRpcClient = categoryServiceRpcClient;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync();
            if (!products.Any())
            {
                return new List<ProductDto>();
            }

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            var categories = await _categoryServiceRpcClient.GetAllCategoriesAsync();

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
                }
            }

            return productDtos;
        }
    }
}
