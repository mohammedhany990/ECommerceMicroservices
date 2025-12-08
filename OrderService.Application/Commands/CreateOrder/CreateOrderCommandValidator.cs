using FluentValidation;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.ShippingAddressId)
                .NotEmpty()
                .WithMessage("Shipping address is required.");

            RuleFor(x => x.ShippingMethodId)
                .NotEmpty()
                .WithMessage("ShippingId method is required.");




        }
    }
}
