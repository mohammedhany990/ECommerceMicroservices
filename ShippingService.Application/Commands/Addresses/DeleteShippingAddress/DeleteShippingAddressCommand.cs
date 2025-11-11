    using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Addresses.DeleteShippingAddress
{
    public class DeleteShippingAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

    }
}
