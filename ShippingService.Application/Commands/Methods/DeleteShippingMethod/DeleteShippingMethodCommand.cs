using MediatR;

namespace ShippingService.Application.Commands.Methods.DeleteShippingMethod
{
    public class DeleteShippingMethodCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
