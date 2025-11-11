using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Shipments.DeleteShipment
{
    public class DeleteShipmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
