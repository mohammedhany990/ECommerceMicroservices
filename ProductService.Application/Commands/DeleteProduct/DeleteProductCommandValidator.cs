using FluentValidation;

namespace ProductService.Application.Commands.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.")
                .Must(id => id != Guid.Empty)
                .WithMessage("ProductId cannot be an empty GUID.");
        }
    }
}
