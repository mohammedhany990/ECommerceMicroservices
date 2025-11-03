using FluentValidation;

namespace ProductService.Application.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 500 characters.");

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
                 .NotEmpty()
                 .WithMessage("Category ID must not be empty.");


        }
    }
}
