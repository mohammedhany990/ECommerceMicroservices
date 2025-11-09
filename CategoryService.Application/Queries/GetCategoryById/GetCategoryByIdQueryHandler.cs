using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Data;
using CategoryService.Domain.Interfaces;
using MediatR;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _repository;

        public GetCategoryByIdQueryHandler(IMapper mapper, IRepository<Category> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return null;
            }
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
