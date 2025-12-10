using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;

namespace ProductService.Application.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IMapper mapper,
            IRepository<Product> repository,
            IFileService fileService,
            CategoryServiceRpcClient categoryServiceRpcClient,
            ILogger<UpdateProductCommandHandler> logger
        )
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
            _categoryServiceRpcClient = categoryServiceRpcClient;
            _logger = logger;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProductCommand for ProductId {ProductId}", request.Id);

            var product = await _repository.GetByIdAsync(request.Id);
            if (product is null)
            {
                _logger.LogWarning("Product with Id {ProductId} not found", request.Id);
                throw new KeyNotFoundException($"Product with Id {request.Id} not found.");
            }

            var category = await _categoryServiceRpcClient.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
            {
                _logger.LogWarning("Invalid Category Id {CategoryId}", request.CategoryId);
                throw new Exception($"Invalid Category Id {request.CategoryId}. The category does not exist.");
            }

            _mapper.Map(request, product);
            product.UpdatedAt = DateTime.UtcNow;

            if (request.ImageBytes != null && !string.IsNullOrEmpty(request.ImageName))
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    _fileService.DeleteFile(product.ImageUrl);
                    _logger.LogInformation("Deleted old image {ImageUrl} for ProductId {ProductId}", product.ImageUrl, request.Id);
                }

                var imageUrl = _fileService.UploadFile(request.ImageBytes, request.ImageName, "Products");
                product.ImageUrl = imageUrl;
                _logger.LogInformation("Uploaded new image {ImageName} for ProductId {ProductId}", request.ImageName, request.Id);
            }

            await _repository.SaveChangesAsync();
            _logger.LogInformation("Product {ProductId} updated successfully", request.Id);

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            productDto.CategoryDescription = category.Description;

            return productDto;
        }


    }
}
