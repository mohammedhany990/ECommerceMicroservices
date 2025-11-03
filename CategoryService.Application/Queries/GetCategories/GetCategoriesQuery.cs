using CategoryService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryService.Application.Queries.GetProducts
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}
