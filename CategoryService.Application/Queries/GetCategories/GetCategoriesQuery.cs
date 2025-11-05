using CategoryService.Application.DTOs;
using MediatR;

namespace CategoryService.Application.Queries.GetProducts
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}
