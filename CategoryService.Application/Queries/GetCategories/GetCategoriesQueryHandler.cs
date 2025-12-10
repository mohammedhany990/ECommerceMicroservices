using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CategoryService.Application.Queries.GetProducts
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IRepository<Category> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoriesQueryHandler> _logger;

        public GetCategoriesQueryHandler(IRepository<Category> repository, IMapper mapper, ILogger<GetCategoriesQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all categories");

            var categories = await _repository.GetAllAsync();

            var result = _mapper.Map<List<CategoryDto>>(categories);

            _logger.LogInformation("Retrieved {Count} categories", result.Count);

            return result;
        }
    }
}
