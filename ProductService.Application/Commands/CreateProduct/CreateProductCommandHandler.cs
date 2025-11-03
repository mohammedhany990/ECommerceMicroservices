using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;

        public CreateProductCommandHandler(
            IMapper mapper,
            IRepository<Product> repository,
            IFileService fileService)
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {

            var product = _mapper.Map<Product>(request);

            if (request.ImageBytes != null && !string.IsNullOrEmpty(request.ImageName))
            {
                var imageUrl = _fileService.UploadFile(request.ImageBytes, request.ImageName, "Products");
                product.ImageUrl = imageUrl;
            }

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }
    }
}
