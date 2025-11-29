using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Messaging;
using ProductService.Infrastructure.Services;

namespace ProductService.Application.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;
        private readonly CategoryServiceRpcClient _categoryServiceRpcClient;

        public CreateProductCommandHandler(
            IMapper mapper,
            IRepository<Product> repository,
            IFileService fileService,
            CategoryServiceRpcClient categoryServiceRpcClient
            )
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
            _categoryServiceRpcClient = categoryServiceRpcClient;
        }
        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryServiceRpcClient.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
                throw new Exception("Invalid Category Id. The category does not exist.");

            var product = _mapper.Map<Product>(request);

            if (request.ImageBytes != null && !string.IsNullOrEmpty(request.ImageName))
            {
                var imageUrl = _fileService.UploadFile(request.ImageBytes, request.ImageName, "Products");
                product.ImageUrl = imageUrl;
            }

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            productDto.CategoryDescription = category.Description;

            return productDto;
        }
    }
}
