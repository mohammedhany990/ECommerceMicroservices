using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                

            //RuleFor(x => x.PaymentMethodId)
            //    .NotEmpty()
            //    .WithMessage("Payment method is required.")
            //    .MaximumLength(100);

            
        }
    }
}
