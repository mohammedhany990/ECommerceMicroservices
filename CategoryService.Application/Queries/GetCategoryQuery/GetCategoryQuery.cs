using CategoryService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryQuery : IRequest<CategoryDto>
    {
        public Guid CategoryId { get; set; }
        public GetCategoryQuery(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
