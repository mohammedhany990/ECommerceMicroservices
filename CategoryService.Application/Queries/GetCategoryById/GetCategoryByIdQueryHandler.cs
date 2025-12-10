using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _repository;
        private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

        public GetCategoryByIdQueryHandler(IMapper mapper, IRepository<Category> repository, ILogger<GetCategoryByIdQueryHandler> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving category by Id: {CategoryId}", request.CategoryId);

            var category = await _repository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}", request.CategoryId);
                return null;
            }

            var result = _mapper.Map<CategoryDto>(category);
            _logger.LogInformation("Category retrieved successfully. CategoryId: {CategoryId}", request.CategoryId);

            return result;
        }
    }
}
