using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;

        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IRepository<Product> repository,
            IFileService fileService,
            ILogger<DeleteProductCommandHandler> logger
        )
        {
            _repository = repository;
            _fileService = fileService;
            _logger = logger;
        }


        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProductCommand for ProductId {ProductId}", request.ProductId);

            var product = await _repository.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                _logger.LogWarning("Product with Id {ProductId} not found", request.ProductId);
                return false;
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                _fileService.DeleteFile(product.ImageUrl);
                _logger.LogInformation("Deleted image file {ImageUrl} for ProductId {ProductId}", product.ImageUrl, request.ProductId);
            }

            await _repository.DeleteAsync(request.ProductId);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Product with Id {ProductId} deleted successfully", request.ProductId);

            return true;
        }

    }
}
