using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CategoryService.Application.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IRepository<Category> _repository;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IRepository<Category> repository, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete category with ID: {CategoryId}", request.CategoryId);

            var category = await _repository.GetByIdAsync(request.CategoryId);
            if (category is null)
            {
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}", request.CategoryId);
                return false;
            }

            await _repository.DeleteAsync(request.CategoryId);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Category deleted successfully. CategoryId: {CategoryId}", request.CategoryId);
            return true;
        }
    }
    
}
