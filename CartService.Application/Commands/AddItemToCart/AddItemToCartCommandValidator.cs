using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
    {
        public AddItemToCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");

            RuleFor(x => x.Item)
                .NotNull()
                .WithMessage("Cart item is required.");

            When(x => x.Item != null, () =>
            {
                RuleFor(x => x.Item.ProductId)
                    .NotEmpty()
                    .WithMessage("ProductId is required.");

                RuleFor(x => x.Item.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero.");

                RuleFor(x => x.Item.UnitPrice)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Price must be greater than or equal to zero.");
            });
        }
    }
}
