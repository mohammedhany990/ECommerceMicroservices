using MediatR;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddressById
{
    public class GetShippingAddressByIdQuery : IRequest<ShippingAddressDto>
    {
        public Guid ID { get; set; }      
        
    }
}
