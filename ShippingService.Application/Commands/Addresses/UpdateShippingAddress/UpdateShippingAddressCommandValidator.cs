using FluentValidation;

namespace ShippingService.Application.Commands.Addresses.UpdateShippingAddress
{
    public class UpdateShippingAddressCommandValidator : AbstractValidator<UpdateShippingAddressCommand>
    {
        public UpdateShippingAddressCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Address Id is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.AddressLine1)
                .NotEmpty().WithMessage("Address line 1 is required.")
                .MaximumLength(200).WithMessage("Address line 1 cannot exceed 200 characters.");

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200).WithMessage("Address line 2 cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

            
        }
    }
}
