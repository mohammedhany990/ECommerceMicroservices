using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
    {
        public GetCategoryQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId must not be empty.")
                .NotEqual(Guid.Empty)
                .WithMessage("CategoryId must be a valid GUID.");
        }
    }
}
