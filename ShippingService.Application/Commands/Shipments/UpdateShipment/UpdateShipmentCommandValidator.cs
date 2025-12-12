using FluentValidation;
using Shared.Enums;

namespace ShippingService.Application.Commands.Shipments.UpdateShipment
{
    public class UpdateShipmentCommandValidator : AbstractValidator<UpdateShipmentCommand>
    {
        public UpdateShipmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Shipment Id is required.");

            RuleFor(x => x.TrackingNumber)
                .MaximumLength(50)
                .WithMessage("Tracking number cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.TrackingNumber));

            RuleFor(x => x.Status)
                 .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<ShipmentStatus>(status, ignoreCase: true, out _))
                 .WithMessage("Invalid shipment status.");


            RuleFor(x => x.DeliveredAt)
                .Must((cmd, deliveredAt) =>
                    !deliveredAt.HasValue || !cmd.ShippedAt.HasValue || deliveredAt.Value >= cmd.ShippedAt.Value)
                .WithMessage("DeliveredAt must be greater than or equal to ShippedAt.");
        }
    }

}
