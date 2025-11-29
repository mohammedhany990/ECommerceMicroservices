using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;
using MediatR;

namespace CategoryService.Application.Queries.GetProducts
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IRepository<Category> _repository;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(IRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllAsync();

            return _mapper.Map<List<CategoryDto>>(categories);

        }
    }
}
