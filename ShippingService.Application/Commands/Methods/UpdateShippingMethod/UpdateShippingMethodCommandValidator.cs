using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Methods.UpdateShippingMethod
{
    public class UpdateShippingMethodCommandValidator : AbstractValidator<UpdateShippingMethodCommand>
    {
        public UpdateShippingMethodCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Cost must be a positive value or zero.");

            RuleFor(x => x.EstimatedDeliveryDays)
                .GreaterThan(0)
                .WithMessage("Estimated delivery days must be greater than zero.");
        }
    }
}
