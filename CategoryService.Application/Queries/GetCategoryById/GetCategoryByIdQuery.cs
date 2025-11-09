using CategoryService.Application.DTOs;
using MediatR;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public Guid CategoryId { get; set; }
        public GetCategoryByIdQuery(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
