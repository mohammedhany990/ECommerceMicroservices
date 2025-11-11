using MediatR;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddresses
{
    public class GetShippingAddressesQuery : IRequest<List<ShippingAddressDto>>
    {
    }
}
