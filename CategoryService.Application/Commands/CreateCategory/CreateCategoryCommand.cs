using CategoryService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryService.Application.Commands.CreateCategory
{
    public class CreateCategoryCommand: IRequest<CategoryDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
