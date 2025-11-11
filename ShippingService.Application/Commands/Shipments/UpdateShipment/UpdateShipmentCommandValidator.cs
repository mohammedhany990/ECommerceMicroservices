using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .MaximumLength(20)
                .WithMessage("Status cannot exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.Status));

            RuleFor(x => x.DeliveredAt)
                .GreaterThanOrEqualTo(x => x.ShippedAt.Value)
                .WithMessage("DeliveredAt must be greater than or equal to ShippedAt.")
                .When(x => x.DeliveredAt.HasValue && x.ShippedAt.HasValue);
        }
    }
}
