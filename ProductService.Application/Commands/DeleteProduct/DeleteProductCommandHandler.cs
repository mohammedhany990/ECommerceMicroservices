using MediatR;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IRepository<Product> _repository;
        private readonly IFileService _fileService;

        public DeleteProductCommandHandler(IRepository<Product> repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductId);
            if (product == null)
                return false;

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                _fileService.DeleteFile(product.ImageUrl);
            }

            await _repository.DeleteAsync(request.ProductId);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
