using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Services;

namespace ProductService.Application.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly CategoryServiceClient _categoryServiceClient;

        public GetProductByIdQueryHandler(IRepository<Product> repository, IMapper mapper, CategoryServiceClient categoryServiceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceClient = categoryServiceClient;
        }
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return null;
            }
            var productDto = _mapper.Map<ProductDto>(product);

            var category = await _categoryServiceClient.GetCategoryByIdAsync(product.CategoryId);

            productDto.CategoryName = category?.Name ?? "Unknown";
            productDto.CategoryDescription = category?.Description ?? "No description available";

            return productDto;
        }
    }
}
