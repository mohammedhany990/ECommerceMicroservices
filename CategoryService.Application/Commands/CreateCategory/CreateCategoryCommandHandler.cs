using CategoryService.Application.DTOs;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CategoryService.Application.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IRepository<Category> _repository;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(IRepository<Category> repository, ILogger<CreateCategoryCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting creation of a new category. Name: {CategoryName}", request.Name);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Category created successfully. CategoryId: {CategoryId}, Name: {CategoryName}", category.Id, category.Name);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return categoryDto;
        }
    }
}
