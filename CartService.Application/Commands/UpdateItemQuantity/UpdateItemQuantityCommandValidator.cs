using FluentValidation;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandValidator : AbstractValidator<UpdateItemQuantityCommand>
    {
        public UpdateItemQuantityCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .NotEqual(0)
                .WithMessage("Quantity cannot be zero. Use positive to increase or negative to decrease.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(-1000, 1000)
                .WithMessage("Quantity must be between -1000 and 1000.");
        }
    }
}
