using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Methods.DeleteShippingMethod
{
    public class DeleteShippingMethodCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
