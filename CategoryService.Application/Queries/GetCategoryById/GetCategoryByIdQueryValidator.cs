using FluentValidation;

namespace CategoryService.Application.Queries.GetCategoryQuery
{
    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId must not be empty.")
                .NotEqual(Guid.Empty)
                .WithMessage("CategoryId must be a valid GUID.");
        }
    }
}
