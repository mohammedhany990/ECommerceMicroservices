using FluentValidation;

namespace ProductService.Application.Commands.UpdateProduct
{

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(200)
                .WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero.");


            RuleFor(x => x.DiscountPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.DiscountPrice.HasValue)
                .WithMessage("Discount price cannot be negative.");

            RuleFor(x => x.QuantityInStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity in stock cannot be negative.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Category Id must be valid.");

        }
    }

}
