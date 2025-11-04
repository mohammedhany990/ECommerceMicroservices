using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Services;

namespace ProductService.Application.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;
        private readonly CategoryServiceClient _categoryServiceClient;

        public UpdateProductCommandHandler(
            IMapper mapper,
            IRepository<Product> repository,
            IFileService fileService,
            CategoryServiceClient categoryServiceClient)
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
            _categoryServiceClient = categoryServiceClient;
        }
        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);
            if (product is null)
                throw new KeyNotFoundException($"Product with Id {request.Id} not found.");

            var category = await _categoryServiceClient.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
                throw new Exception($"Invalid Category Id {request.CategoryId}. The category does not exist.");

            _mapper.Map(request, product);

            product.UpdatedAt = DateTime.UtcNow;

            if (request.ImageBytes != null && !string.IsNullOrEmpty(request.ImageName))
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    _fileService.DeleteFile(product.ImageUrl);
                }

                var imageUrl = _fileService.UploadFile(request.ImageBytes, request.ImageName, "Products");
                product.ImageUrl = imageUrl;
            }

            await _repository.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            productDto.CategoryDescription = category.Description;

            return productDto;
        }

    }
}
