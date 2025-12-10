using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;

namespace ProductService.Application.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IMapper mapper,
            IRepository<Product> repository,
            IFileService fileService,
            CategoryServiceRpcClient categoryServiceRpcClient,
            ILogger<CreateProductCommandHandler> logger 
            )
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
            _categoryServiceRpcClient = categoryServiceRpcClient;
            _logger = logger;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProductCommand for product {ProductName}", request.Name);

            var category = await _categoryServiceRpcClient.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
            {
                _logger.LogWarning("Invalid Category Id {CategoryId} provided", request.CategoryId);
                throw new Exception("Invalid Category Id. The category does not exist.");
            }

            var product = _mapper.Map<Product>(request);

            if (request.ImageBytes != null && !string.IsNullOrEmpty(request.ImageName))
            {
                var imageUrl = _fileService.UploadFile(request.ImageBytes, request.ImageName, "Products");
                product.ImageUrl = imageUrl;

                _logger.LogInformation("Uploaded product image {ImageName}", request.ImageName);
            }

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Product {ProductName} created successfully with Id {ProductId}", product.Name, product.Id);

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            productDto.CategoryDescription = category.Description;

            return productDto;
        }

    }
}
