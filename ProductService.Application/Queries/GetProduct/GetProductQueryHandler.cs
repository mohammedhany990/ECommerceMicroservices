using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;
        private readonly CategoryServiceClient _categoryServiceClient;

        public GetProductQueryHandler(IRepository<Product> repository, IMapper mapper, CategoryServiceClient categoryServiceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryServiceClient = categoryServiceClient;
        }
        public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
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
