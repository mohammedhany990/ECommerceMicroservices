using FluentValidation;
using ShippingService.Application.Commands.Shipments.CreateShipment;
using Shared.Enums;

namespace ShippingService.Application.Commands.Shipments.CreateShipment
{
    public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required.");

            RuleFor(x => x.ShippingAddressId)
                .NotEmpty()
                .WithMessage("ShippingAddressId is required.");

            RuleFor(x => x.ShippingMethodId)
                .NotEmpty()
                .WithMessage("ShippingMethodId is required.");

            RuleFor(x => x.TrackingNumber)
                .MaximumLength(50)
                .WithMessage("Tracking number cannot exceed 50 characters.");
           

            RuleFor(x => x.DeliveredAt)
                .GreaterThanOrEqualTo(x => x.ShippedAt ?? DateTime.MinValue)
                .WithMessage("DeliveredAt must be greater than or equal to ShippedAt.")
                .When(x => x.DeliveredAt.HasValue);
        }
    }
}
